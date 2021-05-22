using UnityEngine;

public class Platform : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 movePosition;

    Rigidbody rb => GetComponent<Rigidbody>();

    float moveSpeed = 20;

    bool inAir = true;
    bool moveDir;
    bool move = true;
    public bool direction;

    #region MONO
    void Start()
    {
        //scale
        transform.localScale = new Vector3(GameManager.instance.currentPlatform.localScale.x, 1,
                           GameManager.instance.currentPlatform.localScale.z);

        //direction
        if (direction)
        {
            startPosition = new Vector3(transform.position.x, transform.position.y, 14);
            movePosition = new Vector3(transform.position.x, transform.position.y, -14);
        }
        else
        {
            startPosition = new Vector3(-14, transform.position.y, transform.position.z);
            movePosition = new Vector3(14, transform.position.y, transform.position.z);
        }
    }
    void Update()
    {
        if (move) Move();
    }

    void OnTriggerEnter() => inAir = false;
    void OnTriggerExit() => inAir = true;
    #endregion

    #region STATE
    void Move()
    {
        if (!moveDir)
            transform.position = Vector3.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);

        if (transform.position == movePosition)
            moveDir = true;
        if (transform.position == startPosition)
            moveDir = false;
    }

    public void Touched()
    {
        move = false;
        if (inAir)
            GameManager.instance.GameOver(rb);
        else
        {
            if (!direction)
            {
                Vector3 distance1 = new Vector3(transform.position.x, 0, 0);
                Vector3 distance2 = new Vector3(GameManager.instance.currentPlatform.position.x, 0, 0);

                if (Vector3.Distance(distance1, distance2) <= 0.35f)
                    AccuratePosition();
                else
                    CutX(transform);
            }
            else
            {
                Vector3 distance1 = new Vector3(0, 0, transform.position.z);
                Vector3 distance2 = new Vector3(0, 0, GameManager.instance.currentPlatform.position.z);

                if (Vector3.Distance(distance1, distance2) <= 0.35f)
                    AccuratePosition();
                else
                    CutZ(transform);
            }
        }
    }

    void AccuratePosition()
    {
        if (!GameManager.instance.pulseEffect)
            GameManager.instance.pulseEffect = true;

        transform.position = new Vector3(GameManager.instance.currentPlatform.position.x,
            transform.position.y, GameManager.instance.currentPlatform.position.z);
        GameManager.instance.NewPlatform(transform);
        StartCoroutine(GameManager.instance.PulseEffect());
        GetComponent<Platform>().enabled = false;
        AudioManager.instance.Combo();
    }

    public bool CutX(Transform victim)
    {
        GameManager.instance.pulseEffect = false;
        bool generalPlatform;
        Vector3 _pos;
        if (transform.position.x < GameManager.instance.currentPlatform.position.x)
        {
            _pos = GameManager.instance.left.position;
            generalPlatform = true;
            //Right
        }
        else
        {
            _pos = GameManager.instance.right.position;
            generalPlatform = false;
            //Left
        }

        Vector3 pos = new Vector3(_pos.x, victim.position.y, victim.position.z);
        Vector3 victimScale = victim.localScale;
        float distance = Vector3.Distance(victim.position, pos);
        if (distance >= victimScale.x / 2) return false;

        Vector3 leftPoint = victim.position - Vector3.right * victimScale.x / 2;
        Vector3 rightPoint = victim.position + Vector3.right * victimScale.x / 2;
        Destroy(victim.gameObject);

        GameObject rightSideObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightSideObj.transform.parent = GameManager.instance.spawnedObjectsParent;
        rightSideObj.transform.position = (rightPoint + pos) / 2;
        float rightWidth = Vector3.Distance(pos, rightPoint);
        rightSideObj.transform.localScale = new Vector3(rightWidth, victimScale.y, victimScale.z);
        rightSideObj.GetComponent<MeshRenderer>().material = ColorManager.instance.groundMaterial;
        rightSideObj.name = "right";
        rightSideObj.tag = "platform";

        GameObject leftSideObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftSideObj.transform.parent = GameManager.instance.spawnedObjectsParent;
        leftSideObj.transform.position = (leftPoint + pos) / 2;
        float leftWidth = Vector3.Distance(pos, leftPoint);
        leftSideObj.transform.localScale = new Vector3(leftWidth, victimScale.y, victimScale.z);
        leftSideObj.GetComponent<MeshRenderer>().material = ColorManager.instance.groundMaterial;
        leftSideObj.name = "left";
        leftSideObj.tag = "platform";

        if (generalPlatform)
        {
            GameManager.instance.right.position = new Vector3(rightSideObj.transform.position.x + rightSideObj.transform.localScale.x / 2,
                                                              0, GameManager.instance.right.position.z);
            GameManager.instance.NewPlatform(rightSideObj.transform);
            leftSideObj.AddComponent<Rigidbody>();
        }
        else
        {
            GameManager.instance.left.position = new Vector3(leftSideObj.transform.position.x - leftSideObj.transform.localScale.x / 2,
                                                              0, GameManager.instance.left.position.z);
            GameManager.instance.NewPlatform(leftSideObj.transform);
            rightSideObj.AddComponent<Rigidbody>();
        }

        AudioManager.instance.Cut();

        return true;
    }
    public bool CutZ(Transform victim)
    {
        GameManager.instance.pulseEffect = false;
        bool generalPlatform;
        Vector3 _pos;
        if (transform.position.z < GameManager.instance.currentPlatform.position.z)
        {
            _pos = GameManager.instance.back.position;
            generalPlatform = true;
            //front
        }
        else
        {
            _pos = GameManager.instance.front.position;
            generalPlatform = false;
            //back
        }


        Vector3 pos = new Vector3(victim.position.x, victim.position.y, _pos.z);
        Vector3 victimScale = victim.localScale;
        float distance = Vector3.Distance(victim.position, pos);
        if (distance >= victimScale.z / 2) return false;

        Vector3 leftPoint = victim.position - Vector3.forward * victimScale.z / 2;
        Vector3 rightPoint = victim.position + Vector3.forward * victimScale.z / 2;
        Destroy(victim.gameObject);

        GameObject frontSideObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontSideObj.transform.parent = GameManager.instance.spawnedObjectsParent;
        frontSideObj.transform.position = (rightPoint + pos) / 2;
        float rightWidth = Vector3.Distance(pos, rightPoint);
        frontSideObj.transform.localScale = new Vector3(victimScale.x, victimScale.y, rightWidth);
        frontSideObj.GetComponent<MeshRenderer>().material = ColorManager.instance.groundMaterial;
        frontSideObj.name = "front";
        frontSideObj.tag = "platform";

        GameObject backSideObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backSideObj.transform.parent = GameManager.instance.spawnedObjectsParent;
        backSideObj.transform.position = (leftPoint + pos) / 2;
        float leftWidth = Vector3.Distance(pos, leftPoint);
        backSideObj.transform.localScale = new Vector3(victimScale.x, victimScale.y, leftWidth);
        backSideObj.GetComponent<MeshRenderer>().material = ColorManager.instance.groundMaterial;
        backSideObj.name = "back";
        backSideObj.tag = "platform";

        if (generalPlatform)
        {
            GameManager.instance.front.position = new Vector3(GameManager.instance.front.position.x, 0,
                                                  frontSideObj.transform.position.z + frontSideObj.transform.localScale.z / 2);
            GameManager.instance.NewPlatform(frontSideObj.transform);
            backSideObj.AddComponent<Rigidbody>();
        }
        else
        {
            GameManager.instance.back.position = new Vector3(GameManager.instance.front.position.x, 0,
                                                      backSideObj.transform.position.z - backSideObj.transform.localScale.z / 2);
            GameManager.instance.NewPlatform(backSideObj.transform);
            frontSideObj.AddComponent<Rigidbody>();
        }

        AudioManager.instance.Cut();

        return true;
    }
    #endregion
}
