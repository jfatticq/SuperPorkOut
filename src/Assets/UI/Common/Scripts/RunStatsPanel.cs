using System.Collections.Generic;
using SuperPorkOut.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace SuperPorkOut.UI
{
    [UxmlElement]
    public partial class RunStatsPanel : VisualElement
    {
        [UxmlAttribute]
        public string sceneName { get; set; } = "Endless";

        [UxmlAttribute]
        public string title { get; set; } = "Best Runs";

        [UxmlAttribute]
        public bool autoLoad { get; set; } = true;

        private Label titleLabel;
        private VisualElement tableContainer;
        private Label emptyLabel;

        public RunStatsPanel()
        {
            BuildStructure();
            RegisterCallback<AttachToPanelEvent>(OnAttach);
        }

        private void BuildStructure()
        {
            style.paddingTop = 8;
            style.paddingBottom = 8;
            style.paddingLeft = 8;
            style.paddingRight = 8;
            style.marginTop = 8;
            style.marginBottom = 8;

            titleLabel = new Label(title);
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.fontSize = 18;
            titleLabel.style.marginBottom = 6;
            titleLabel.style.unityTextAlign = TextAnchor.UpperCenter;
            titleLabel.style.color = new Color(0.31f, 0.20f, 0.08f);
            Add(titleLabel);

            tableContainer = new VisualElement();
            Add(tableContainer);

            emptyLabel = new Label("No runs recorded yet.");
            emptyLabel.style.fontSize = 13;
            emptyLabel.style.unityTextAlign = TextAnchor.UpperCenter;
            emptyLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            emptyLabel.style.color = new Color(0.31f, 0.20f, 0.08f, 0.6f);
            emptyLabel.style.paddingTop = 12;
            emptyLabel.style.paddingBottom = 12;
            Add(emptyLabel);
        }

        private void OnAttach(AttachToPanelEvent evt)
        {
            if (autoLoad)
                Refresh();
        }

        public void Refresh()
        {
            Refresh(sceneName);
        }

        public void Refresh(string scene)
        {
            sceneName = scene;
            titleLabel.text = title;

            var runs = RunStatsStore.GetTopRuns(sceneName);
            PopulateTable(runs);
        }

        private void PopulateTable(List<RunStatsEntry> runs)
        {
            tableContainer.Clear();

            if (runs == null || runs.Count == 0)
            {
                emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            emptyLabel.style.display = DisplayStyle.None;

            var header = MakeRow("#", "Distance", "Time", "Pickups", "Date", isHeader: true);
            tableContainer.Add(header);

            for (int i = 0; i < runs.Count; i++)
            {
                var entry = runs[i];
                var row = MakeRow(
                    (i + 1).ToString(),
                    $"{entry.distanceTraveled:0.0}m",
                    FormatTime(entry.timeElapsed),
                    FormatPickups(entry),
                    entry.dateTime ?? "",
                    isHeader: false
                );
                tableContainer.Add(row);
            }
        }

        private static VisualElement MakeRow(
            string rank, string distance, string time, string pickups, string date,
            bool isHeader)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.paddingTop = 3;
            row.style.paddingBottom = 3;
            row.style.borderBottomWidth = isHeader ? 2 : 1;
            row.style.borderBottomColor = isHeader
                ? new Color(0, 0, 0, 0.3f)
                : new Color(0, 0, 0, 0.15f);

            var fontStyle = isHeader ? FontStyle.Bold : FontStyle.Normal;

            row.Add(MakeCell(rank, fontStyle, flexGrow: 0.3f, minWidth: 24));
            row.Add(MakeCell(distance, fontStyle));
            row.Add(MakeCell(time, fontStyle));
            row.Add(MakeCell(pickups, fontStyle));
            row.Add(MakeCell(date, fontStyle));

            return row;
        }

        private static Label MakeCell(string text, FontStyle fontStyle, float flexGrow = 1f, float minWidth = 0f)
        {
            var cell = new Label(text);
            cell.style.fontSize = 12;
            cell.style.flexGrow = flexGrow;
            cell.style.flexBasis = 0;
            cell.style.unityTextAlign = TextAnchor.UpperCenter;
            cell.style.color = new Color(0.24f, 0.16f, 0.06f);
            cell.style.overflow = Overflow.Hidden;
            cell.style.unityFontStyleAndWeight = fontStyle;
            if (minWidth > 0)
                cell.style.minWidth = minWidth;
            return cell;
        }

        private static string FormatTime(float seconds)
        {
            if (seconds < 0f) seconds = 0f;
            int total = Mathf.FloorToInt(seconds);
            int mins = total / 60;
            int secs = total % 60;
            return $"{mins:00}:{secs:00}";
        }

        private static string FormatPickups(RunStatsEntry entry)
        {
            var parts = new List<string>(4);

            if (entry.carrotCount > 0) parts.Add($"C:{entry.carrotCount}");
            if (entry.cabbageCount > 0) parts.Add($"B:{entry.cabbageCount}");
            if (entry.tomatoCount > 0) parts.Add($"T:{entry.tomatoCount}");
            if (entry.otherCount > 0) parts.Add($"O:{entry.otherCount}");

            return parts.Count > 0 ? string.Join(" ", parts) : "0";
        }
    }
}
