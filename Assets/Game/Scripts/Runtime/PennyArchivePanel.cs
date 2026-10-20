using System;
using System.Collections.Generic;
using PennyUniversity;
using UnityEngine;
using UnityEngine.UI;

namespace PennyUniversity.Runtime
{
    /// <summary>
    /// Unity uGUI archive window. Only titles are visible until a paid selection.
    /// A visit always consumes one fee and grants exactly one verified record.
    /// </summary>
    public sealed class PennyArchivePanel : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform titleListRoot;
        [SerializeField] private Button titleButtonPrefab;
        [SerializeField] private Text statusText;
        [SerializeField] private Text documentText;
        [SerializeField] private Button closeButton;

        private PennyGameState state;
        private readonly List<ArchiveRecord> catalog = new List<ArchiveRecord>();
        private int entranceFee;
        private bool isOpen;

        public void Initialize(PennyGameState gameState, IEnumerable<ArchiveRecord> authoredRecords, int fee)
        {
            if (gameState == null) throw new ArgumentNullException(nameof(gameState));
            if (authoredRecords == null) throw new ArgumentNullException(nameof(authoredRecords));
            if (fee <= 0) throw new ArgumentOutOfRangeException(nameof(fee));
            if (titleListRoot == null || titleButtonPrefab == null || panelRoot == null)
                throw new InvalidOperationException("Assign panelRoot, titleListRoot and titleButtonPrefab.");

            state = gameState;
            entranceFee = fee;
            catalog.Clear();
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var record in authoredRecords)
            {
                if (record == null || string.IsNullOrWhiteSpace(record.Id) ||
                    string.IsNullOrWhiteSpace(record.Title) || string.IsNullOrWhiteSpace(record.Content) ||
                    !ids.Add(record.Id))
                    throw new ArgumentException("Records need unique IDs, titles and factual contents.");
                catalog.Add(record);
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
            if (state == null) throw new InvalidOperationException("Call Initialize first.");
            isOpen = true;
            panelRoot.SetActive(true);
            if (documentText != null) documentText.text = "";
            RenderTitles();
        }

        public void Close()
        {
            isOpen = false;
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void RenderTitles()
        {
            for (int i = titleListRoot.childCount - 1; i >= 0; i--)
                Destroy(titleListRoot.GetChild(i).gameObject);

            bool allowed = !state.Finished &&
                state.ArchiveVisits < PennyGameState.MaximumArchiveVisits &&
                state.Money >= entranceFee;
            if (statusText != null)
                statusText.text = "입장료: " + entranceFee + " | 보유금: " + state.Money +
                    " | 방문: " + state.ArchiveVisits + "/" + PennyGameState.MaximumArchiveVisits +
                    (allowed ? "\n제목을 선택하면 입장료를 지불하고 해당 기록 한 권만 읽습니다." :
                    "\n입장 불가: 자금 부족, 방문 제한 또는 게임 종료");

            foreach (var record in catalog)
            {
                ArchiveRecord selected = record;
                var button = Instantiate(titleButtonPrefab, titleListRoot);
                var label = button.GetComponentInChildren<Text>();
                if (label != null) label.text = selected.Title;
                button.interactable = allowed;
                button.onClick.AddListener(() => SelectRecord(selected));
            }
        }

        private void SelectRecord(ArchiveRecord selected)
        {
            if (!isOpen || state == null) return;
            if (!state.TryReadArchive(selected, entranceFee, out var fact))
            {
                RenderTitles();
                return;
            }

            // Only reveal the full factual text after the transaction succeeds.
            if (documentText != null) documentText.text = fact.Title + "\n\n" + fact.Text;
            if (statusText != null)
                statusText.text = "기록 열람 완료. 입장료가 지불되었으며 이번 방문은 종료되었습니다.";

            // One selection per visit: remove all title buttons until reopened.
            for (int i = titleListRoot.childCount - 1; i >= 0; i--)
                Destroy(titleListRoot.GetChild(i).gameObject);
        }
    }
}