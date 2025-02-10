using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public Spell[] spells = new Spell[4];
    public KeyCode[] spellKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };

    private Spell selectedSpell;
    private GameObject spellIndicator;
    private Camera mainCamera;

    public float autoAttackRange = 5f;
    public GameObject rangeIndicatorPrefab;
    private GameObject rangeIndicator;

    public float autoAttackCooldown = 1.0f;
    private float lastAutoAttackTime;

    private float[] spellCooldowns = { 5f, 7f, 10f, 15f }; // Q, W, E, R cooldowns
    private float[] lastCastTimes = new float[4];

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        for (int i = 0; i < spells.Length; i++)
        {
            if (Input.GetKeyDown(spellKeys[i]) && spells[i] != null)
            {
                if (Time.time >= lastCastTimes[i] + spellCooldowns[i])
                {
                    SelectSpell(spells[i]);
                    lastCastTimes[i] = Time.time; // Start cooldown
                }
                else
                {
                    Debug.Log($"Spell {spellKeys[i]} is on cooldown!");
                }
            }
        }

        if (selectedSpell != null)
        {
            UpdateSpellIndicator();
            UpdateRangeIndicator();

            if (Input.GetMouseButtonDown(0))
            {
                CastSelectedSpell();
            }
        }

        // Auto attack only when right-clicking an enemy
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Enemy") && Time.time >= lastAutoAttackTime + autoAttackCooldown)
                {
                    AutoAttack(hit.collider.gameObject);
                    lastAutoAttackTime = Time.time; // Start auto-attack cooldown
                }
            }
        }
    }

    private void SelectSpell(Spell spell)
    {
        selectedSpell = spell;

        if (spellIndicator != null) Destroy(spellIndicator);
        if (rangeIndicator != null) Destroy(rangeIndicator);

        if (selectedSpell.spellPrefab != null)
        {
            spellIndicator = Instantiate(selectedSpell.spellPrefab);
            spellIndicator.GetComponent<Collider>().enabled = false;
        }

        if (selectedSpell is WSpell && rangeIndicatorPrefab != null)
        {
            rangeIndicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity);
            rangeIndicator.transform.localScale = new Vector3(autoAttackRange * 2, 1, autoAttackRange * 2);
        }
    }

    private void UpdateSpellIndicator()
    {
        if (spellIndicator != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                spellIndicator.transform.position = hit.point;
            }
        }
    }

    private void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.position = transform.position;
        }
    }

    private void CastSelectedSpell()
    {
        if (selectedSpell == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            float distance = Vector3.Distance(transform.position, hit.point);

            // W Spell: Drop on the ground
            if (selectedSpell is WSpell)
            {
                Instantiate(selectedSpell.spellPrefab, hit.point, Quaternion.Euler(90, 0, 0));
            }
            // E Spell: Drop at player but ignore player's collider
            else if (selectedSpell is ESpell)
            {
                Vector3 spawnPosition = GetGroundPosition(transform.position + Vector3.up * 2f);
                Instantiate(selectedSpell.spellPrefab, spawnPosition, Quaternion.Euler(90, 0, 0));
            }
            // Q Spell: Shoot from the player to the target
            else
            {
                GameObject spellObj = Instantiate(selectedSpell.spellPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
                Spell spellInstance = spellObj.GetComponent<Spell>();
                if (spellInstance != null)
                {
                    spellInstance.Cast(hit.point);
                }
            }

            Debug.Log($"Spell {selectedSpell.name} cast at: " + hit.point);
        }

        if (spellIndicator != null) Destroy(spellIndicator);
        if (rangeIndicator != null) Destroy(rangeIndicator);

        selectedSpell = null;
    }

    private Vector3 GetGroundPosition(Vector3 position)
    {
        Ray ray = new Ray(position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }
        return position; // Fallback in case no ground is hit
    }

    private void AutoAttack(GameObject target)
    {
        Debug.Log($"Auto Attack performed on {target.name}!");
        // Implement auto attack logic here
    }
}
