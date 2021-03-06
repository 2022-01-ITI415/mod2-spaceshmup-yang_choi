using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a "shield" type to allow a shield power-up.
/// Items marked [NI] below are Not Implemented in the IGDPD book.
/// </summary>
public enum WeaponType
{
    none, // The default / no weapons
    blaster, // A simple blaster
    spread, // Two shots simultaneously
    phaser, // [NI] Shots that move in waves NOT MADE
    missile, // [NI] Homing missiles
    laser, // [NI] Damage over time NOT MADE
    shield, // Raise shieldLevel
    shotgun, // Random spread/pellet
    minigun, // shoots tons of bullets with tiny spread and low damage 
    flame,
    troll, // A weapon meant to troll the player. You'll know when you get it.
    mine // Its a mine -> no projectile movement + low ROF + tons of damage.
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties
/// of a specific weapon in the Inspector. The Main class has
/// an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; // Letter to show on the power-up
    public Color color = Color.white; // Color of Collar & power-up
    public GameObject projectilePrefab; // Prefab for projectiles
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; // Amount of damage caused
    public float continuousDamage = 0; // Damage per second (Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20; // Speed of projectiles
    //public float rotateSpeed = 200f; // rotate speed
    public AudioClip soundEffect; // the sound for the PowerUp type
}
public class Weapon : MonoBehaviour {
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Time last shot was fired
    private Renderer collarRend;
    public Transform target;
    public float rotateSpeed = 200f;


    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Call SetType() for the default _type of WeaponType.none
        SetType(_type);

        // Dynamically create an anchor for all Projectiles
        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;


        lastShotTime = 0; // You can fire immediately after _type is set.
    }

    public void Fire()
    {
        Debug.Log("Weapon Fired:" + gameObject.name);
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;


        // If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:

                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;

            case WeaponType.spread:
                
                p = MakeProjectile(); // Make middle Projectile
                p.rigid.velocity = vel;

                p = MakeProjectile(); // Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make middle right Projectile
                p.transform.rotation = Quaternion.AngleAxis(5, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make middle left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-5, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                break;

            case WeaponType.shotgun:

                int baseProjectileNum = 10;         // base number of Projectile

                float randomNumber = Random.Range(0.0f, 5.0f);
                int number = (int) randomNumber; // makes a random number from 0 to 5

                int totalProjectiles = baseProjectileNum + number;

                for (int i = 0; i < totalProjectiles + 1; i++) 
                {
                    // makes a random number from -16 to 16 the spread
                    float numberAng = Random.Range(-16f, 16f);
                    p = MakeProjectile();
                    //def.damageOnHit = 200;          // damage per Projectile
                    p.transform.rotation = Quaternion.AngleAxis(numberAng, Vector3.back);
                    p.rigid.velocity = p.transform.rotation * vel;
                }

                break;

            case WeaponType.minigun:
                
                // small spread will make ROF fast with low damage
                float Ang = Random.Range(-4f, 4f);
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(Ang, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;

            case WeaponType.missile:
                // target = GameObject.FindGameObjectWithTag("Enemy").transform;
                // p = MakeProjectile();
                // Vector2 direction = (Vector2)target.position;
                // direction.Normalize();
                // float rotateAmount = Vector3.Cross(direction, transform.up).z;
                // p.angularVelocity = rotateAmount * rotateSpeed;
                // p.transform.rotation = Quaternion.AngleAxis(Ang, Vector3.back);

                
                // p.rigid.velocity = vel;
                
                break;

            case WeaponType.flame:
                
                p = MakeProjectile(); // Make middle Projectile
                p.rigid.velocity = vel;

                p = MakeProjectile(); // Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(5, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make middle right Projectile
                p.transform.rotation = Quaternion.AngleAxis(2, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-5, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile(); // Make middle left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-2, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                break;
            case WeaponType.troll:

                p = MakeProjectile();
                p.rigid.velocity = -vel;
                break;
            case WeaponType.mine:

                p = MakeProjectile();
                break;    
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if(transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return p;
    }
}
