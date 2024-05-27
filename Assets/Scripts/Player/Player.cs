using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


// Authors: Marc Federspiel
public class Player : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveForce = 30.0f;
    [SerializeField] private float maxMoveVelocity = 10.0f;


    [Header("References")]
    [SerializeField] private MolotovCocktailMock molotov = null;
    [SerializeField] private Material deadMaterial;


    [Header("Debug Info")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private Vector2 moveInput;
    private TargetEventChecker deathChecker;
    private PlayerControls controls;
    private Rigidbody rb;

    private Transform defaultWeaponTransform;
    private Transform molotovTransform;

    void Awake()
    {

        rb = GetComponent<Rigidbody>();

        controls = new PlayerControls();

        moveInput = Vector2.zero;

        Transform[] transforms = GetComponentsInChildren<Transform>();

        foreach (Transform t in transforms)
        {

            switch (t.gameObject.name)
            {

                case "MolotovTransform": molotovTransform = t; break;
                case "GunTransform": defaultWeaponTransform = t; break;
            }
        }

        deathChecker = GetComponent<TargetEventChecker>();
    }


    void OnEnable()
    {

        controls.BattleControls.Move.performed += onMovePerformed;
        controls.BattleControls.Move.canceled += onMoveCanceled;
        controls.BattleControls.Shoot.performed += onShootPerformed;
        controls.BattleControls.Throw.performed += onThrowPerformed;
        controls.Enable();
    }

    void OnDisable()
    {

        controls.BattleControls.Move.performed -= onMovePerformed;
        controls.BattleControls.Move.canceled -= onMoveCanceled;
        controls.BattleControls.Shoot.performed -= onShootPerformed;
        controls.BattleControls.Throw.performed -= onThrowPerformed;
        controls.Disable();
    }

    public void onMovePerformed(InputAction.CallbackContext c)
    {


        moveInput = c.ReadValue<Vector2>();
    }

    public void onMoveCanceled(InputAction.CallbackContext c)
    {


        moveInput = c.ReadValue<Vector2>();
    }

    public void onShootPerformed(InputAction.CallbackContext c)
    {

        if (weapon == null) return;

        weapon.shoot();

        if (weapon.isEmpty())
        {

            dropWeapon();
        }
    }

    public void onThrowPerformed(InputAction.CallbackContext c)
    {


        if (molotov == null) return;

        molotov.transform.SetParent(null);
        molotov.shoot();

        molotov = null;
    }


    void Start()
    {


        weapon = null;
    }

    void Update()
    {


        setLookRotation();


        if (deathChecker.getIsDeath())
        {

            die();
        }
    }

    void FixedUpdate()
    {


        move();
    }

    private void move()
    {

        if (moveInput.x == 0.0f)
        {

            rb.velocity = new Vector3(0.0f, rb.velocity.y, rb.velocity.z);
        }
        if (moveInput.y == 0.0f)
        {

            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0.0f);

        }

        if (rb.velocity.magnitude > maxMoveVelocity)
        {

            rb.velocity = rb.velocity.normalized * maxMoveVelocity;
        }

        Vector3 moveVec = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized * moveForce;
        rb.AddForce(moveVec);
    }

    private void setLookRotation()
    {

        Ray ray = Camera.main.ScreenPointToRay(
              new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Ground")) ||
            Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))
        {

            float t = (transform.position.y - ray.origin.y) / ray.direction.y;

            Vector3 position = ray.origin + t * ray.direction;

            Quaternion rot = Quaternion.LookRotation(position - transform.position, Vector3.up);
            rot.x = 0.0f;
            rot.z = 0.0f;

            transform.rotation = rot;
        }
    }

    public void pickupWeapon(Weapon weapon)
    {

        GameObject g = weapon.gameObject;

        /*
        if (molotov == null && (weapon as MolotovCocktailMock) != null)
        {


            g.transform.position = molotovTransform.position;
            g.transform.rotation = molotovTransform.rotation;
            g.transform.SetParent(molotovTransform);
            molotov = g.GetComponent<MolotovCocktailMock>();
        }
        else if ((weapon as MolotovCocktailMock) == null)
        {

            g.transform.position = defaultWeaponTransform.position;
            g.transform.rotation = defaultWeaponTransform.rotation;
            g.transform.SetParent(defaultWeaponTransform);


            this.weapon = weapon;
        }
        */
    }


    private void dropWeapon()
    {

        weapon.drop();

        weapon = null;
    }


    public void die()
    {


        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = deadMaterial;

        StartCoroutine("hide", 2.0f);
    }

    public IEnumerator hide(float duration)
    {

        yield return new WaitForSeconds(duration);

        GetComponent<MeshRenderer>().enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {

        int layer = 1 << collision.gameObject.layer;
        if (layer == LayerMask.GetMask("Wall"))
        {



        }

    }


    public bool canPickupWeapon()
    {

        return weapon == null;
    }

    public bool canPickupMolotov()
    {

        return molotov == null;
    }

}
