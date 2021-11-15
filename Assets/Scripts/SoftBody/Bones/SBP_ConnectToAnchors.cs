using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SBP_ConnectToAnchors : MonoBehaviour
{
    [Header("Collider References")]
    [SerializeField]
    bool addCollider;

    [SerializeField]
    float collderSizeX = 0.001f, colliderSizeZ = 0.001f;

    [Tooltip("The following field is just a reference for the inspector and not used in script")]
    [SerializeField]
    string boneRegion = "";

    [Header("Transform References")]
    [SerializeField]
    Transform[] _bones;

    [SerializeField]
    int neighborsPerBone = 1;

    [SerializeField]
    Transform[] _anchors;


    #region Rigidbody Variables
    [Header("Bone Rigidbody variables")]
    [SerializeField]
    float _rbMass;

    [SerializeField]
    float _rbDrag, _rbAngularDrag;

    [SerializeField]
    bool _rbUseGravity, _rbIsKinematic;

    [SerializeField]
    RigidbodyInterpolation _rbInterpolate = RigidbodyInterpolation.None;

    [SerializeField]
    CollisionDetectionMode _rbCollisionDetection = CollisionDetectionMode.Discrete;

    //ToDo: Consider adding Constraints here
    #endregion

    #region Spring Variables
    [Header("Anchor Spring variables")]
    [SerializeField]
    bool connectToAnchors = true;

    [SerializeField]
    float _sjAnchorSpring = 1000f, _sjAnchorDamper = 0f, _sjAnchorMinDistance = 0, _sjAnchorMaxDistance = 0, _sjAnchorTolerance = 0.025f;

    //ToDo: Consider adding other variables here

    [Header("Bone2Bone Spring variables")]
    [SerializeField]
    bool connectBoneToBone = true;

    [SerializeField]
    float _sjBoneSpring = 1000f, _sjBoneDamper = 0f, _sjBoneMinDistance = 0, _sjBoneMaxDistance = 0, _sjBoneTolerance = 0.025f;

    //ToDo: Consider adding other variables here
    [Header("Own position Spring variables")]
    [SerializeField]
    bool connectToOwnPosition = false;

    [SerializeField]
    float _sjOwnSpring = 1000f, _sjOwnDamper = 0f, _sjOwnMinDistance = 0, _sjOwnMaxDistance = 0, _sjOwnTolerance = 0.025f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Initialize RB on each anchor
        foreach (Transform anchor in _anchors) {
            if (!anchor.GetComponent<Rigidbody>()) {
                Rigidbody rb = anchor.gameObject.AddComponent<Rigidbody>();

                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        //Initialize RB on each bone
        foreach (Transform bone in _bones)
        {
            if (!bone.GetComponent<Rigidbody>())
            {
                Rigidbody rb = bone.gameObject.AddComponent<Rigidbody>();

                rb.mass = _rbMass;
                rb.drag = _rbDrag;
                rb.angularDrag = _rbAngularDrag;
                rb.useGravity = _rbUseGravity;
                rb.isKinematic = _rbIsKinematic;
                rb.interpolation = _rbInterpolate;
                rb.collisionDetectionMode = _rbCollisionDetection;

                if (addCollider) {
                    BoxCollider col = bone.gameObject.AddComponent<BoxCollider>();

                    Transform boneEnd = bone.GetChild(0).GetChild(0).GetComponent<Transform>();  //Find the tip of the bone (Middle of bone -> Root of bone -> End of bone)

                    //Vector3 sizeOffset = new Vector3(col.size.x / bone.transform.localScale.x, col.size.y / bone.transform.localScale.y, col.size.z / bone.transform.localScale.z);
                    //col.center = boneEnd.localPosition / 2;
                    //col.center = (boneEnd.position - bone.position) / 2;
                    //col.center = new Vector3(col.center.x / bone.transform.localScale.x, col.center.y / bone.transform.localScale.y, col.center.z / bone.transform.localScale.z);

                    col.size = new Vector3(collderSizeX, boneEnd.localPosition.y*.9f, colliderSizeZ);
                }
            }
        }

        List<Transform> processedBones = new List<Transform>();


        for (int i = 0; i < _bones.Length; i++)
        {
            List<Transform> closestNeighbors = new List<Transform>();
            bool sorted = false;

            for (int j = 0; j < _bones.Length; j++)
            {
                if (j == i)
                {

                }
                /*
                else if (processedBones.Contains(_bones[j]))
                {
                    //neighborsPerBone -= 1;
                }
                */
                else if (closestNeighbors.Count < neighborsPerBone)
                {
                    closestNeighbors.Add(_bones[j]);
                }
                else
                {
                    if (!sorted)
                    {
                        closestNeighbors = sortNeighborsByDistance(closestNeighbors, _bones[i].position, _bones[i].name);

                        sorted = true;
                    }

                    for (int k = 0; k < closestNeighbors.Count; k++)
                    {
                        float neighborDistance = (closestNeighbors[k].position - _bones[i].position).magnitude;
                        float potentialNeighborDistance = (_bones[j].position - _bones[i].position).magnitude;

                        if (potentialNeighborDistance < neighborDistance)
                        {
                            closestNeighbors.Insert(k, _bones[j]);

                            closestNeighbors.RemoveAt(closestNeighbors.Count - 1);

                            break;
                        }
                    }
                }
            }

            //Add Springs 
            //to anchors
            if (connectToAnchors)
            {
                foreach (Transform anchor in _anchors)
                {
                    SpringJoint spring = _bones[i].gameObject.AddComponent<SpringJoint>();

                    spring.spring = _sjAnchorSpring;
                    spring.damper = _sjAnchorDamper;
                    spring.minDistance = _sjAnchorMinDistance;
                    spring.maxDistance = _sjAnchorMaxDistance;
                    spring.tolerance = _sjAnchorTolerance;

                    spring.connectedBody = anchor.GetComponent<Rigidbody>();
                }
            }

            if (connectBoneToBone)
            {
                //to neighbor bones
                foreach (Transform neighborBone in closestNeighbors)
                {
                    SpringJoint spring = _bones[i].gameObject.AddComponent<SpringJoint>();

                    spring.connectedBody = neighborBone.GetComponent<Rigidbody>();

                    spring.spring = _sjBoneSpring;
                    spring.damper = _sjBoneDamper;
                    spring.minDistance = _sjBoneMinDistance;
                    spring.maxDistance = _sjBoneMaxDistance;
                    spring.tolerance = _sjBoneTolerance;
                }
            }

            if (connectToOwnPosition)
            {
                //and to it's root position in local space
                SpringJoint spring = _bones[i].gameObject.AddComponent<SpringJoint>();

                spring.spring = _sjOwnSpring;
                spring.damper = _sjOwnDamper;
                spring.minDistance = _sjOwnMinDistance;
                spring.maxDistance = _sjOwnMaxDistance;
                spring.tolerance = _sjOwnTolerance;
            }

            processedBones.Add(_bones[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    List<Transform> sortNeighborsByDistance(List<Transform> transforms, Vector3 bonePosition, string boneName)
    {
        List<Vector3> vecs = new List<Vector3>();

        //Extract positions out of transforms List
        foreach (Transform trans in transforms)
        {
            vecs.Add(trans.position);
        }

        DistanceComparer distComp = new DistanceComparer(bonePosition);

        vecs.Sort(distComp);

        //Convert sorted positions back to transforms and Sort them accordingly

        List<Transform> sortedTransforms = new List<Transform>();

        foreach (Vector3 vec in vecs)
        {
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
