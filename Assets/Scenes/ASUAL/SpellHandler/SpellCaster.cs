using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public spells[] spellList;
    private Spell selectedSpell;
    private int selectedSpellNum;
    private Camera mainCamera;
    public SpellCooldownUI splCldwnUi;

    private float[] lastCastTimes = new float[4];
    public GameObject rangeIndicatorPrefab;
    private GameObject activeRangeIndicator;
    private float currentSpellRange = 0f;

    private SpellCooldownUI cooldownUI;
    private bool isSwitchMode = false;

    public Transform attackSpawnPoint;
    public LayerMask unitLayerMask;

    private void Start()
    {
        mainCamera = Camera.main;

        if (GameStateManager.Instance.selectedUnit == null)
        {
            GameStateManager.Instance.selectedUnit = FindFirstObjectByType<SelectableUnit>();
            Debug.Log($"✅ Default unit set to {GameStateManager.Instance.selectedUnit?.gameObject.name}");
        }
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
                        isSwitchMode = true;
                        selectedSpell = spellList[i].spell;
                        Debug.Log("Switch mode activated. Click on a unit to switch.");
                        spellList[i].spell.Cast(transform.position);
                    }
                    else
                    {
                        selectedSpell = spellList[i].spell;
                        selectedSpellNum = i;
                        ShowRangeIndicator(spellList[i].range);
                    }

                    SpellCooldownUI.instance?.StartCooldown(i, spellList[i].spellCooldowns);
                }
            }
        }

        if (selectedSpell != null && Input.GetMouseButtonDown(0))
        {
            if (isSwitchMode)
                AttemptCharacterSwitch();
            else
                CastSelectedSpell();

            HideRangeIndicator();
        }
    }

    private void AttemptCharacterSwitch()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayerMask))
        {
            HealthAndStamina hp = hit.collider.transform.root.GetComponent<HealthAndStamina>();
            if (hp == null) return;

            if (hp.isStunned) GameStateManager.Instance.TransferToTarget(transform, hit.collider.transform.root);
        }

        selectedSpell = null;
        isSwitchMode = false;
    }

    private void ToggleCharacterControl(GameObject character, bool isActive)
    {
        
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

            selectedSpell.Initialize(this);
            selectedSpell.Cast(castPosition);
        }

        selectedSpell = null;
        isSwitchMode = false;
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
