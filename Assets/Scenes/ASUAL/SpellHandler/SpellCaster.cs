using Spells;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public Spell[] spells = new Spell[4];
    public KeyCode[] spellKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };
    private Spell selectedSpell;
    private int selectedSpellNum;
    private Camera mainCamera;
    public SpellCooldownUI splCldwnUi;

    private float[] spellCooldowns = { 5f, 7f, 10f, 15f };
    private float[] lastCastTimes = new float[4];

    public GameObject rangeIndicatorPrefab; 
    private GameObject activeRangeIndicator;
    private float currentSpellRange = 0f;

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
                    lastCastTimes[i] = Time.time;

                    if (spells[i] is ESpell)
                    {
                        spells[i].Cast(transform.position);
                    }
                    else
                    {
                        selectedSpell = spells[i];
                        selectedSpellNum = i;
                        ShowRangeIndicator(GetSpellRange(spells[i]));
                    }
                }
                else
                {
                    Debug.Log($"Spell {spellKeys[i]} is on cooldown!");
                }
            }
        }

        if (selectedSpell != null && Input.GetMouseButtonDown(0))
        {
            CastSelectedSpell();
            HideRangeIndicator();
            splCldwnUi.StartCooldown(selectedSpellNum);
        }
    }

    private void ShowRangeIndicator(float range)
    {
        if (activeRangeIndicator != null) Destroy(activeRangeIndicator);

        
        if (selectedSpell is ESpell) return;

        activeRangeIndicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity);
        activeRangeIndicator.transform.localScale = new Vector3(range * 2, 0.1f, range * 2);
        currentSpellRange = range;
    }

    private void HideRangeIndicator()
    {
        if (activeRangeIndicator != null) Destroy(activeRangeIndicator);
    }

    private float GetSpellRange(Spell spell)
    {
        if (spell is QSpell) return 10f; 
        if (spell is WSpell) return 5f; 
        if (spell is HomingSpell) return 12f;
        return 0f;
    }

    private void CastSelectedSpell()
    {
        if (selectedSpell == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Vector3 castPosition = hit.point;

            
            float distance = Vector3.Distance(transform.position, castPosition);
            if (distance > currentSpellRange)
            {
                Vector3 direction = (castPosition - transform.position).normalized;
                castPosition = transform.position + direction * currentSpellRange;
            }

            selectedSpell.Cast(castPosition);
            Debug.Log($"Spell {selectedSpell.name} cast at: {castPosition}");
        }

        selectedSpell = null;
    }
}
