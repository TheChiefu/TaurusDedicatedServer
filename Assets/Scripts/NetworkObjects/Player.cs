using System.Collections;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    
    
    
    [Header("Player Information")]
    public int id = -1;
    public string username = string.Empty;
    public int teamID = -1;

    [Header("Primary Data")]
    [SerializeField] private bool isBot = false;
    public float health = 0f;
    public float shield = 0f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxShield = 100f;

    [Header("Cosmestic")]
    public int modelIndex = -1;
    public int materialIndex = -1;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpSpeed = 5f;
    public float sprintMultipler = 2.5f;

    [Tooltip("Inputs given by either player input or CPU player")]
    public bool[] inputs; //Refer to either SeverHandle or Help text file to determine which input index matches what action

    [Header("Shooting")]
    [SerializeField] private Transform shootOrigin;
    [SerializeField] private float throwingForce = 600f;

    [Tooltip("Things that change when interacting with server. 'Non-static' properties")]
    [Header("Current Modifiables")]
    [SerializeField] private int itemAmount = 0;
    [SerializeField] private int maxItemAmount = 3;


    //Private
    private float gravity = -9.81f;
    
    private float yVelocity = 0;
    private CharacterController controller;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        gravity = Physics.gravity.y;
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;


        //TESTING PURPOSES - DO NOT SET TEAMID HERE NORMALLY
        teamID = 0;
    }

    public void Initialize(int _id, string _username, int _modelIndex, int _materialIndex)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        modelIndex = _modelIndex;
        materialIndex = _materialIndex;

        inputs = new bool[6];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            return;
        }


        //Directional
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        


        Move(_inputDirection);
    }










    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {

        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;

        //Sprint Toggle
        if (inputs[5]) _moveDirection *= (moveSpeed * sprintMultipler); 
        else _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }


    /// <summary>
    /// Given a Vector3 direction, player shoots in that direction
    /// </summary>
    /// <param name="_viewDirection"></param>
    public void Shoot(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
            else if (_hit.collider.CompareTag("Enemy"))
            {
                _hit.collider.GetComponent<Enemy>().TakeDamage(50f);
            }
        }
    }


    /// <summary>
    /// Throw's item depending on given Vector3 direction
    /// </summary>
    /// <param name="_viewDirection"></param>
    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (itemAmount > 0)
        {
            itemAmount--;
            LevelManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwingForce, id);
        }
    }

    /// <summary>
    /// Deals damage from given amount to player
    /// </summary>
    /// <param name="_damage"></param>
    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;

            //Spawn at random position
            Transform[] spawnPoints = LevelManager.instance.spawnPoints;
            int rand = Random.Range(0, spawnPoints.Length);
            transform.position = spawnPoints[rand].position;

            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }

    /// <summary>
    /// Adds item to player's useable inventory
    /// </summary>
    /// <returns></returns>
    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;
    }
}