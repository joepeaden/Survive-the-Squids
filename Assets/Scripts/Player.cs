using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class Player : MonoBehaviour
    {
        public static Player instance;
        public float moveForce;
        public List<Enemy> enemiesInRange = new List<Enemy>();
        public List<CharacterBody> CharacterBodies => characterBodies;

        public CharacterStatsData statsData;
        public Dictionary<CharacterInfo.CharTraits, TraitData> traitEnumToData = new Dictionary<CharacterInfo.CharTraits, TraitData>();
        public TraitData quickTraitData;
        public TraitData preciseTraitData;
        public TraitData toughTraitData;
        public TraitData brutalTraitData;
        public TraitData smartTraitData;

        [SerializeField]
        // character bodies (to be filled in with character info)
        private List<CharacterBody> characterBodies;

        // backend info about the characters
        private List<CharacterInfo> characters = new List<CharacterInfo>();
        private List<WeaponData> ownedWeapons = new List<WeaponData>();
        //private int characterIndex = 0;
        private CharacterBody controlledCharacter;
        private Rigidbody2D rb;
        private GameManager gameManager;

        private void Awake()
        {
            instance = this;

            traitEnumToData[CharacterInfo.CharTraits.Quick] = quickTraitData;
            traitEnumToData[CharacterInfo.CharTraits.Precise] = preciseTraitData;
            traitEnumToData[CharacterInfo.CharTraits.Tough] = toughTraitData;
            traitEnumToData[CharacterInfo.CharTraits.Brutal] = brutalTraitData;
            traitEnumToData[CharacterInfo.CharTraits.Smart] = smartTraitData;
        }

        private void Start()
        {
            gameManager = GameManager.instance;
            rb = GetComponent<Rigidbody2D>();
            gameManager.OnGameStart.AddListener(HandleGameStart);

            ownedWeapons.Add(gameManager.weapons[0]);
            HandleGameStart();
        }

        private void OnDestroy()
        {
            gameManager.OnGameStart.RemoveListener(HandleGameStart);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CharacterBody.ManualAimEnabled = !CharacterBody.ManualAimEnabled;
            }
        }

        private void FixedUpdate()
        {
            if (!gameManager.inMenu)
            {
                UpdateMovement();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Enemy")
            {
                enemiesInRange.Add(collision.GetComponent<Enemy>());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Enemy")
            {
                enemiesInRange.Remove(collision.GetComponent<Enemy>());
            }
        }

        public Enemy GetEnemy()
        {
            if (enemiesInRange.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, enemiesInRange.Count);
            return enemiesInRange[randomIndex];
        }

        private void HandleGameStart()
        {
            for (int i = 0; i < characterBodies.Count; i++)
            {
                characterBodies[i].Reset();
            }

            characters.Clear();
            for (int i = 0; i < 9; i++)
            {
                CharacterInfo charInfo = new CharacterInfo(statsData);
                characters.Add(charInfo);

                AssignCharacterInfoToBody(charInfo, characterBodies[i]);
            }

            //SwitchCharacters(0);

            transform.position = Vector2.zero;

            // remove all weapons but the initial one
            ownedWeapons.RemoveRange(1, ownedWeapons.Count - 1);
        }

        private void AssignCharacterInfoToBody(CharacterInfo charInfo, CharacterBody body)
        {
            body.SetCharacter(charInfo);
        }

        private void UpdateMovement()
        {
            Vector2 direction = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction += (Vector2.up);
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += (-Vector2.up);
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += (Vector2.right);
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += (-Vector2.right);
            }

            rb.AddForce(direction * moveForce);
            //rb.velocity = newVelocity * moveForce;
        }

        /// <summary>
        /// Update the charbodies with new values from charinfo (like after level up)
        /// </summary>
        public void UpdateCharBodies()
        {
            foreach (CharacterBody body in characterBodies)
            {
                body.SetCharacter(body.CharInfo);
            }
        }

        public void PickupWeapon(WeaponData weapon)
        {
            ownedWeapons.Add(weapon);

            controlledCharacter.SetWeapon(weapon);
            controlledCharacter.CharInfo.weaponData = weapon;

            //gameManager.UpdateCharacterUI(controlledCharacter);
        }
    }
}