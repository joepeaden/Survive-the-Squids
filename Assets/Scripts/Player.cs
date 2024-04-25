using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class Player : MonoBehaviour
    {
        public const int MAX_CHARACTERS = 8;

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

        [SerializeField] private Transform characterParent;
        /// <summary>
        /// Characters actually being played in the game (not inactive bodies)
        /// </summary>
        public List<CharacterBody> ActiveCharacters;
        private Dictionary<string, int> activeCharactersIndex = new Dictionary<string, int>();

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

        public int HitPoints = 10;

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

            for (int i = 0; i < characterParent.childCount; i++)
            {
                CharacterParent charPar = characterParent.GetChild(i).GetComponent<CharacterParent>();
                characterBodies.Add(charPar.charBody);
            }
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

        public void GetHit(int damage)
        {
            HitPoints--;

            if (HitPoints <= 0)
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

            PlayerBar.Instance.HandleHit();
        }

        public void UpdateSamples(int num)
        {
            playerSamples += num;
            gameManager.UpdateSamples(playerSamples);
        }


        public void CheckGameOver()
        {
            //if (ActiveCharacters.Count <= 0)
            //{
            //    gameManager.GameOver();
            //}
        }

        public void AddCharacterNoReturn()
        {
            AddCharacter();
        }

        //pu

        public CharacterBody ReplaceCharacter(CharacterInfo charInfo)
        {
            if (activeCharactersIndex.ContainsKey(charInfo.ID))
            {
                int index = activeCharactersIndex[charInfo.ID];
                RemoveCharacter(charInfo);
                return AddCharacter(index);
            }

            return null;
        }

        public void RemoveCharacter(CharacterInfo charInfo)
        {
            int index = 0;
            if (activeCharactersIndex.ContainsKey(charInfo.ID))
            {
                index = activeCharactersIndex[charInfo.ID];
            }
            else
            {
                Debug.LogWarning("Character with ID " + charInfo.ID + " not found!");
                return;
            }

            ActiveCharacters[index].Disable();
            ActiveCharacters.Remove(ActiveCharacters[index]);
            activeCharactersIndex.Remove(charInfo.ID);

            SetPositions();
        }

        public CharacterBody AddCharacter(int repIndex = -1)
        {
            CharacterInfo charInfo = new CharacterInfo(statsData);


            if (repIndex != -1)
            {
                CharacterBody charBody = characterBodies[repIndex]; 
                charBody.SetCharacter(charInfo);
                charBody.SetBodyActive(true);
                ActiveCharacters.Insert(repIndex, charBody);
                activeCharactersIndex.Add(charInfo.ID, repIndex);

                // gonna change this

                SetPositions();

                return charBody;
            }
            else
            {
                int index = 0;
                foreach (CharacterBody charBody in characterBodies)
                {
                    if (!charBody.isActiveAndEnabled)
                    {
                        charBody.SetCharacter(charInfo);
                        charBody.SetBodyActive(true);

                        ActiveCharacters.Add(charBody);
                        activeCharactersIndex.Add(charInfo.ID, index);

                        // gonna change this

                        SetPositions();

                        //switch (ActiveCharacters.Count)
                        //{
                        //    case 2:
                        //        amntOfChars = amntOfChars / 2;
                        //        break;
                        //}

                        //RegeneratePositions();

                        return charBody;
                    }

                    index++;
                }
            }

            Debug.LogWarning("No characters left to add!");

            return null;
        }

        void SetPositions()
        {
            List<Vector2> characterPositions = null;
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
                    break;
            }

            for (int i = 0; i < ActiveCharacters.Count && characterPositions != null; i++)
            {
                ActiveCharacters[i].transform.parent.localPosition = characterPositions[i];
            }
        }

        //void RegeneratePositions()
        //{
        //    int numGameObjects = ActiveCharacters.Count;
        //    float angleStep = 2f * Mathf.PI / numGameObjects;

        //    for (int i = 0; i < numGameObjects; i++)
        //    {
        //        // Generate a position within the circle while checking for minimum distance
        //        Vector2 position = GetRandomPositionWithMinDistance(i, angleStep);

        //        // Set the position of the game object
        //        ActiveCharacters[i].transform.parent.position = position;
        //    }
        //}

        //Vector2 GetRandomPositionWithMinDistance(int index, float angleStep)
        //{
        //    float minDistanceSqr = minDistance * minDistance;
        //    float angle = index * angleStep;
        //    float x = radius * Mathf.Cos(angle);
        //    float y = radius * Mathf.Sin(angle);
        //    Vector2 position = new Vector2(x, y);

        //    // Check minimum distance from previous game objects
        //    for (int j = 0; j < index; j++)
        //    {
        //        Vector2 prevPosition = (Vector2)ActiveCharacters[j].transform.position;
        //        if (Vector2.SqrMagnitude(position - prevPosition) < minDistanceSqr)
        //        {
        //            // If too close, adjust the position
        //            float offsetAngle = Random.Range(0f, 2f * Mathf.PI);
        //            float offsetRadius = Random.Range(minDistance, 2 * minDistance); // Ensure new position respects minDistance
        //            position = prevPosition + new Vector2(offsetRadius * Mathf.Cos(offsetAngle), offsetRadius * Mathf.Sin(offsetAngle));
        //            // Restart loop to recheck distance with previous objects
        //            j = -1;
        //        }
        //    }

        //    return position;
        //}


        //public float radius;
        //public float minDistance;
        //public void RegeneratePositions()
        //{
        //    int numGameObjects = ActiveCharacters.Count;

        //    for (int i = 0; i < numGameObjects; i++)
        //    {
        //        float angle = Random.Range(0f, 2f * Mathf.PI);
        //        float r = Random.Range(0f, radius);
        //        float x = r * Mathf.Cos(angle);
        //        float y = r * Mathf.Sin(angle);

        //        ActiveCharacters[i].transform.parent.position = new Vector3(x, y, 0f);
        //    }

            //List<Vector2> setPositions = new List<Vector2>();

            //for (int i = 0; i < ActiveCharacters.Count; i++)
            //{
            //    int iterations = 0;
            //    Vector2 newPos;
            //    bool found = false;
            //    do
            //    {
            //        iterations++;

            //        newPos = new Vector2(Random.Range(-groupRange, groupRange), Random.Range(-groupRange, groupRange));
            //        foreach (Vector2 pos in setPositions)
            //        {
            //            if (newPos.x - pos.x < minSep || newPos.y - pos.y < minSep)
            //            {
            //                found = false;
            //            }
            //            else
            //            {
            //                found = true;
            //            }
            //        }
            //    } while (iterations < 50 && !found);
            //    Debug.Log(iterations);

            //    setPositions.Add(newPos);
            //    ActiveCharacters[i].transform.parent.localPosition = newPos;
            //}
        //}

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
            activeCharactersIndex.Add(charInfo.ID, 0);

            playerSamples = 0;

            transform.position = Vector2.zero;
            hitCircle.transform.localScale = new Vector3(1f, 1f);

            PlayerBar.Instance.Initialize();
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

            newMoveDirection.Normalize();

            //rb.AddForce(newMoveDirection * moveForce);

            rb.velocity = newMoveDirection * moveForce;

            if (newMoveDirection != Vector2.zero)
            {
                moveDirection = newMoveDirection;
            }
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