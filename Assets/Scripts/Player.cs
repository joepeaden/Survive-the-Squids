using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class Player : MonoBehaviour
    {
        public static Player instance;
        public float moveForce;

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

        public Transform hitCircle;

        public List<Vector2> positions2Chars = new List<Vector2>();
        public List<Vector2> positions3Chars = new List<Vector2>();
        public List<Vector2> positions4Chars = new List<Vector2>();
        public List<Vector2> positions5Chars = new List<Vector2>();
        public List<Vector2> positions6Chars = new List<Vector2>();
        public List<Vector2> positions7Chars = new List<Vector2>();
        public List<Vector2> positions8Chars = new List<Vector2>();

        //public List<Vector2> positions9Chars = new List<Vector2>();

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

        int hitPoints = 100;
        public void GetHit(int damage)
        {
            hitPoints--;

            if (hitPoints <= 0)
            {
                gameManager.GameOver();
            }
            else
            {
                foreach (CharacterBody b in ActiveCharacters)
                {
                    b.GetHit(damage);
                }
            }
        }

        public void UpdateSamples(int num)
        {
            playerSamples += num;
            gameManager.UpdateSampleUI(playerSamples);
        }


        public void CheckGameOver()
        {
            //if (ActiveCharacters.Count <= 0)
            //{
            //    gameManager.GameOver();
            //}
        }

        public CharacterBody  AddCharacter()
        {
            CharacterInfo charInfo = new CharacterInfo(statsData);

            foreach (CharacterBody charBody in characterBodies)
            {
                if (!charBody.isActiveAndEnabled)
                {
                    charBody.SetCharacter(charInfo);
                    charBody.SetBodyActive(true);
                    ActiveCharacters.Add(charBody);

                    // gonna change this

                    List<Vector2> characterPositions;
                    switch (ActiveCharacters.Count)
                    {
                        case 2:
                            characterPositions = positions2Chars;
                            hitCircle.transform.localScale = new Vector3(1.2f, 1.2f);
                            break;
                        case 3:
                            characterPositions = positions3Chars;
                            hitCircle.transform.localScale = new Vector3(1.5f, 1.5f);
                            break;
                        case 4:
                            characterPositions = positions4Chars;
                            hitCircle.transform.localScale = new Vector3(1.8f, 1.8f);
                            break;
                        case 5:
                            characterPositions = positions5Chars;
                            hitCircle.transform.localScale = new Vector3(2f, 2f);
                            break;
                        case 6:
                            characterPositions = positions6Chars;
                            break;
                        case 7:
                            characterPositions = positions7Chars;
                            hitCircle.transform.localScale = new Vector3(2.2f, 2.2f);
                            break;
                        case 8:
                            characterPositions = positions8Chars;
                            break;
                        //case 9:
                        //    characterPositions = positions9Chars;
                        //    break;
                        default:
                            return charBody;
                    }

                    for (int i = 0; i < ActiveCharacters.Count; i++)
                    {
                        ActiveCharacters[i].transform.parent.localPosition = characterPositions[i];
                    }

                    return charBody;
                }
            }

            Debug.LogWarning("No characters left to add!");

            return null;
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
            hitCircle.transform.localScale = new Vector3(1f, 1f);
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