using Spells;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public spells[] spellList;
    private Spell selectedSpell;
    private int selectedSpellNum;
    private Camera mainCamera;
    public SpellCooldownUI splCldwnUi;

    public GameObject rangeIndicatorPrefab;
    private GameObject activeRangeIndicator;
    private float currentSpellRange = 0f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        for (int i = 0; i < spellList.Length; i++)
        {
            if (Input.GetKeyDown(spellList[i].keyCode) && spellList[i].spell != null)
            {
                if (Time.time >= spellList[i].lastCastTimes + spellList[i].spellCooldowns)
                {
                    spellList[i].lastCastTimes = Time.time;

                    if (spellList[i].spell is ESpell)
                    {
                        spellList[i].spell.Cast(transform.position);
                    }
                    else
                    {
                        selectedSpell = spellList[i].spell;
                        selectedSpellNum = i;
                        ShowRangeIndicator(spellList[i].range);
                    }
                }
                else
                {
                    Debug.Log($"Spell {spellList[i].keyCode} is on cooldown!");
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

        activeRangeIndicator = Instantiate(rangeIndicatorPrefab, transform);
        activeRangeIndicator.transform.localScale = new Vector3(range * 2, 0.1f, range * 2);
        currentSpellRange = range;
    }

    private void HideRangeIndicator()
    {
        if (activeRangeIndicator != null) Destroy(activeRangeIndicator);
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

    [System.Serializable]
    public class spells
    {
        public Spell spell;
        public KeyCode keyCode;
        public float spellCooldowns;
        public float range;
        [HideInInspector]public float lastCastTimes;
    }
}
