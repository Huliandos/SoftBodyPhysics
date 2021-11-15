using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_SpringMassBones_Cube : MonoBehaviour
{
    [SerializeField]
    Transform[] _bones;

    [SerializeField]
    float _spring = 10f, _damper = .2f; 

    GameObject _centerPoint;

    [SerializeField]
    bool _sphereColliderOnBones;

    [Header("Test")]
    [SerializeField]
    bool applyPhyisicsMat = false;
    [SerializeField]
    PhysicMaterial physicsMat;

    // Start is called before the first frame update
    void Start()
    {
        //Create a center point for all bones to attach Springs to
        _centerPoint = new GameObject("center");
        _centerPoint.transform.parent = transform;
        _centerPoint.transform.position = transform.position;
        Rigidbody centerPointRB = _centerPoint.AddComponent<Rigidbody>();
        centerPointRB.constraints = RigidbodyConstraints.FreezeAll;

        List<Transform> processedBones = new List<Transform>();

        //Add Basics Components to each bone
        foreach (Transform bone in _bones) {
            bone.gameObject.AddComponent<Rigidbody>();

            Collider col;

            if(_sphereColliderOnBones) col = bone.gameObject.AddComponent<SphereCollider>();
            else col = bone.gameObject.AddComponent<BoxCollider>();

            if (applyPhyisicsMat) col.material = physicsMat;

            Physics.IgnoreCollision(GetComponent<Collider>(), col, true);

            //Distance between bone position and surface of Mesh's Collider
            //float distanceToSurface = (GetComponent<Collider>().ClosestPoint(bone.position) - bone.position).magnitude;   //Doesn't work! ToDo: Find way to automatically calculate that
            
            if (_sphereColliderOnBones) ((SphereCollider)col).radius = .06f;
            else ((BoxCollider)col).size = new Vector3(.1175f, .1175f, .1175f);
        }

        for (int i=0; i < _bones.Length; i++)
        {
            List<Transform> closestNeighbors = new List<Transform>();
            int neighborsNeeded = 3;

            bool sorted = false;
            
            for (int j = 0; j < _bones.Length; j++)
            {
                if (j == i)
                {
                    //if (_bones[i].name.Contains("BBL_Bone"))
                    //{
                    //    Debug.Log("Same Bone with " + _bones[j]);
                    //}
                }
                else if (processedBones.Contains(_bones[j]))
                {
                    //neighborsNeeded -= 1;

                    //if (_bones[i].name.Contains("BBL_Bone"))
                    //{
                    //    Debug.Log("Processed bone " + _bones[j]);
                    //}
                }
                else if (closestNeighbors.Count < neighborsNeeded)
                {
                    closestNeighbors.Add(_bones[j]);

                    //if (_bones[i].name.Contains("BBL_Bone"))
                    //{
                    //    Debug.Log("Initially added " + _bones[j]);
                    //}
                }
                else
                {
                    if (!sorted) {
                        closestNeighbors = sortNeighborsByDistance(closestNeighbors, _bones[i].position, _bones[i].name);

                        sorted = true;
                    }
                    
                    for (int k = 0; k < closestNeighbors.Count; k++)
                    {
                        float neighborDistance = (closestNeighbors[k].position - _bones[i].position).magnitude;
                        float potentialNeighborDistance = (_bones[j].position - _bones[i].position).magnitude;

                        if (potentialNeighborDistance < neighborDistance)
                        {
                            if (_bones[i].name.Contains("BBR_Bone"))
                            {
                                /*
                                Debug.Log("Before: ");
                                foreach (Transform trans in closestNeighbors)
                                {
                                    Debug.Log(trans);
                                }
                                */
                            }
                            closestNeighbors.Insert(k, _bones[j]);

                            if (_bones[i].name.Contains("BBR_Bone"))
                            {
                                /*
                                Debug.Log("After Insertion: ");
                                foreach (Transform trans in closestNeighbors)
                                {
                                    Debug.Log(trans);
                                }
                                */
                            }
                            closestNeighbors.RemoveAt(closestNeighbors.Count - 1);
                        }
                    }
                }
            }
            //Add Springs 
            //to middle
            SpringJoint springJoint = _bones[i].gameObject.AddComponent<SpringJoint>();

            springJoint.connectedBody = _centerPoint.GetComponent<Rigidbody>();
            springJoint.spring = _spring;
            springJoint.damper = _damper;

            //to neighbor bones
            foreach (Transform neighborBone in closestNeighbors) {
                springJoint = _bones[i].gameObject.AddComponent<SpringJoint>();

                springJoint.connectedBody = neighborBone.GetComponent<Rigidbody>();
                springJoint.spring = _spring;
                springJoint.damper = _damper;
            }

            processedBones.Add(_bones[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Transform> sortNeighborsByDistance(List<Transform> transforms, Vector3 bonePosition, string boneName) {
        List<Vector3> vecs = new List<Vector3>();

        //Extract positions out of transforms List
        foreach (Transform trans in transforms) {
            vecs.Add(trans.position);
        }

        DistanceComparer distComp = new DistanceComparer(bonePosition);

        vecs.Sort(distComp);

        //Convert sorted positions back to transforms and Sort them accordingly

        List<Transform> sortedTransforms = new List<Transform>();

        foreach (Vector3 vec in vecs) {
            foreach (Transform trans in transforms)
            {
                if (trans.position == vec)
                {
                    sortedTransforms.Add(trans);
                    break;
                }
            }
        }

        /*
        Debug.Log("---" + boneName + "---");
        Debug.Log("Before");
        foreach (Transform trans in transforms)
        {
            Debug.Log(trans + " magnitude: " + (trans.position - bonePosition).magnitude);
        }
        Debug.Log("After");
        foreach (Transform trans in sortedTransforms)
        {
            Debug.Log(trans + " magnitude: " + (trans.position - bonePosition).magnitude);
        }
        */

        /*
        transforms.Sort((Transform trans1, Transform trans2) =>
            (trans1 - bonePosition).magnitude == (trans2 - bonePosition).magnitude
                ? 0
                : trans1.PartName == null
                    ? -1
                    : trans2.PartName == null
                        ? 1
                        : trans1.PartName.CompareTo(trans2.PartName));

        if ((positionA - compareTo).magnitude == (positionB - compareTo).magnitude) return 0;
        else if ((positionA - compareTo).magnitude < (positionB - compareTo).magnitude) return 1;
        return -1;
        */

        /*
        Transform[] transformsCopy = new Transform[transforms.Count];
        transforms.CopyTo(transformsCopy);

        for (int i = 0; i < transforms.Count; i++) {
            for (int j = 0; j < transforms.Count; j++) {
                if (i == j) { }
                else if ((transforms[i].position - bonePosition).magnitude > (transforms[j].position - bonePosition).magnitude) {
                    if (debugLog)
                    {
                        Debug.Log("Before in loop");
                        Debug.Log(transforms[i]);
                        Debug.Log(transforms[j]);
                    }

                    //Transform placeholderTrans = transforms[i];
                    //transforms[i] = transforms[j];
                    //transforms[j] = placeholderTrans;

                    Transform placeholderTrans = transformsCopy[i];
                    transformsCopy[i] = transforms[j];
                    transformsCopy[j] = placeholderTrans;

                }
            }
        }

        transforms = new List<Transform>();
        transforms.AddRange(transformsCopy);
        if (debugLog)
        {
            Debug.Log("After in loop");
            for (int i = 0; i < transforms.Count; i++)
            {
                Debug.Log(transforms[i]);
            }
        }
        */

        return sortedTransforms;
    }

    class DistanceComparer : IComparer<Vector3>
    {
        Vector3 _compareTo;

        public DistanceComparer(Vector3 compareTo)
        {
            _compareTo = compareTo;
        }

        public int Compare(Vector3 positionA, Vector3 positionB)
        {
            if ((positionA - _compareTo).magnitude == (positionB - _compareTo).magnitude) return 0;
            else if ((positionA - _compareTo).magnitude > (positionB - _compareTo).magnitude) return 1;
            return -1;
        }
    }
}

