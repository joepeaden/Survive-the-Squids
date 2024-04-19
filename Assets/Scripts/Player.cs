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
        /// <summary>
        /// Characters actually being played in the game (not inactive bodies)
        /// </summary>
        public List<CharacterBody> ActiveCharacters;

        private Rigidbody2D rb;
        private GameManager gameManager;

        [HideInInspector]
        public int playerSamples;

        public Vector2 MoveDirection => moveDirection;
        Vector2 moveDirection;

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

        public void UpdateSamples(int num)
        {
            playerSamples += num;
            gameManager.UpdateSampleUI(playerSamples);
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

        public void CheckGameOver()
        {
            if (ActiveCharacters.Count <= 0)
            {
                gameManager.GameOver();
            }
        }

        public void  AddCharacter()
        {
            CharacterInfo charInfo = new CharacterInfo(statsData);

            foreach (CharacterBody charBody in characterBodies)
            {
                if (!charBody.isActiveAndEnabled)
                {
                    charBody.SetCharacter(charInfo);
                    charBody.SetBodyActive(true);
                    ActiveCharacters.Add(charBody);

                    break;
                }
            }
        }

        private void HandleGameStart()
        {
            for (int i = 0; i < characterBodies.Count; i++)
            {
                characterBodies[i].Reset();
            }
            ActiveCharacters.Clear();

            CharacterInfo charInfo = new CharacterInfo(statsData);
            characterBodies[0].SetCharacter(charInfo);
            characterBodies[0].SetBodyActive(true);
            ActiveCharacters.Add(characterBodies[0]);

            playerSamples = 0;

            transform.position = Vector2.zero;
        }

        private void UpdateMovement()
        {
            Vector2 newMoveDirection = Vector2.zero;

            if (Input.GetKey(KeyCode.W))
            {
                newMoveDirection += (Vector2.up);
            }
            if (Input.GetKey(KeyCode.S))
            {
                newMoveDirection += (-Vector2.up);
            }
            if (Input.GetKey(KeyCode.D))
            {
                newMoveDirection += (Vector2.right);
            }
            if (Input.GetKey(KeyCode.A))
            {
                newMoveDirection += (-Vector2.right);
            }

            rb.AddForce(newMoveDirection * moveForce);

            if (newMoveDirection != Vector2.zero)
            {
                moveDirection = newMoveDirection;
            }
            //rb.velocity = newVelocity * moveForce;
        }

        /// <summary>
        /// Update the charbodies with new values from charinfo (like after level up)
        /// </summary>
        public void UpdateCharBodies()
        {
            foreach (CharacterBody body in characterBodies)
            {
                if (body.isActiveAndEnabled)
                {
                    body.SetCharacter(body.CharInfo);
                }
            }
        }
    }
}