using System;
using PennyUniversity;
using UnityEngine;
using UnityEngine.UI;

namespace PennyUniversity.Runtime
{
    /// <summary>
    /// Simple Unity uGUI notebook. Displays evidence separately from editable hypotheses.
    /// Never displays hidden scenario identity or political influence scores.
    /// </summary>
    public sealed class PennyNotebookPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform entryListRoot;
        [SerializeField] private Button entryButtonPrefab;
        [SerializeField] private Text detailText;
        [SerializeField] private Text hypothesesText;
        [SerializeField] private InputField hypothesisInput;
        [SerializeField] private Button addHypothesisButton;
        [SerializeField] private Button closeButton;

        private PennyNotebook notebook;
        private NotebookFilter currentFilter = NotebookFilter.All;

        public void Initialize(PennyNotebook source)
        {
            notebook = source ?? throw new ArgumentNullException(nameof(source));
            if (panelRoot == null || entryListRoot == null || entryButtonPrefab == null)
                throw new InvalidOperationException("Assign the panel root, list root and entry prefab.");
            if (addHypothesisButton != null)
            {
                addHypothesisButton.onClick.RemoveListener(AddHypothesisFromInput);
                addHypothesisButton.onClick.AddListener(AddHypothesisFromInput);
            }
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
                closeButton.onClick.AddListener(Close);
            }
            Close();
        }

        public void Open()
        {
            if (notebook == null) throw new InvalidOperationException("Call Initialize first.");
            panelRoot.SetActive(true);
            Refresh();
        }

        public void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void ShowAll() { currentFilter = NotebookFilter.All; Refresh(); }
        public void ShowClaims() { currentFilter = NotebookFilter.UnverifiedClaims; Refresh(); }
        public void ShowFacts() { currentFilter = NotebookFilter.VerifiedFacts; Refresh(); }

        public void Refresh()
        {
            if (notebook == null || entryListRoot == null) return;
            for (int i = entryListRoot.childCount - 1; i >= 0; i--)
                Destroy(entryListRoot.GetChild(i).gameObject);

            foreach (var entry in notebook.GetEntries(currentFilter))
            {
                EvidenceEntry selected = entry;
                var button = Instantiate(entryButtonPrefab, entryListRoot);
                var label = button.GetComponentInChildren<Text>();
                if (label != null)
                    label.text = (entry.VerifiedFact ? "[확인된 사실] " : "[미확인 주장] ") +
                        (entry.Title ?? "제목 없음");
                button.onClick.AddListener(() => ShowEntry(selected));
            }
            RefreshHypotheses();
        }

        private void ShowEntry(EvidenceEntry entry)
        {
            if (detailText == null) return;
            string source = entry.Source == EvidenceSource.RoyalArchive ? "왕립 기록보관소" :
                entry.Source == EvidenceSource.Newspaper ? "신문" : "토론";
            detailText.text = (entry.VerifiedFact ? "확인된 사실" : "검증되지 않은 주장") +
                " | 출처: " + source + "\n\n" + entry.Title + "\n\n" + entry.Text;
        }

        public void AddHypothesisFromInput()
        {
            if (notebook == null || hypothesisInput == null) return;
            if (!notebook.TryAddHypothesis(hypothesisInput.text, out _)) return;
            hypothesisInput.text = "";
            RefreshHypotheses();
        }

        private void RefreshHypotheses()
        {
            if (hypothesesText == null || notebook == null) return;
            var builder = new System.Text.StringBuilder("나의 가설\n");
            foreach (var hypothesis in notebook.Hypotheses)
                builder.Append('#').Append(hypothesis.Id).Append("  ").Append(hypothesis.Text).Append('\n');
            hypothesesText.text = builder.ToString();
        }
    }
}