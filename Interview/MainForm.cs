using Interview.Core;
using System.Globalization;

namespace Interview.WinForms;

// Controls are created in code so the whole UI can be reviewed in one file.
public sealed class MainForm : Form
{
    private readonly TabControl tabs = new() { Dock = DockStyle.Fill };

    public MainForm()
    {
        Text = "C# Interview (Apisit Swatchin)";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1000, 740);
        MinimumSize = new Size(820, 620);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 11);
        BackColor = Color.FromArgb(245, 247, 251);
        Controls.Add(tabs);
        var header = new Label
        {
            Text = "Pre-Interview Question Windows Developer",
            Dock = DockStyle.Top, Height = 90, Padding = new Padding(18, 12, 0, 0),
            ForeColor = Color.White, BackColor = Color.FromArgb(30, 48, 75)
        };
        Controls.Add(header);
        BuildBrackets(); BuildSort(); BuildAutocomplete(); BuildRoman(); BuildDigits(); BuildTribonacci();
    }

    private FlowLayoutPanel Page(string title, string description)
    {
        var page = new TabPage(title) { BackColor = Color.White };
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, AutoScroll = true, Padding = new Padding(20)
        };
        page.Controls.Add(panel); tabs.TabPages.Add(page);
        Caption(panel, description);
        return panel;
    }

    private static void Caption(FlowLayoutPanel panel, string text)
    {
        panel.Controls.Add(new Label { Text = text, AutoSize = true,
            MaximumSize = new Size(720, 0), Margin = new Padding(0, 8, 0, 6) });
    }

    private static TextBox Input(FlowLayoutPanel panel, string label, string value, bool multiline = false)
    {
        Caption(panel, label);
        var box = new TextBox { Text = value, Width = 710, Multiline = multiline,
            Height = multiline ? 90 : 32, ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None,
            Margin = new Padding(0, 0, 0, 6), AccessibleName = label };
        panel.Controls.Add(box); return box;
    }

    private static void OutputAndButton(FlowLayoutPanel panel, string label, Func<string> execute)
    {
        var button = new Button { Text = label, AutoSize = true, Padding = new Padding(14, 6, 14, 6),
            BackColor = Color.FromArgb(35, 83, 163), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 12, 0, 8) };
        panel.Controls.Add(button);
        Caption(panel, "ผลลัพธ์ (เลือกข้อความเพื่อคัดลอกได้)");
        var output = new TextBox { ReadOnly = true, Multiline = true, Width = 710, Height = 125,
            ScrollBars = ScrollBars.Vertical, BackColor = Color.FromArgb(244, 247, 252),
            AccessibleName = "ผลลัพธ์" };
        panel.Controls.Add(output);
        button.Click += (_, _) =>
        {
            try { output.ForeColor = Color.DarkGreen; output.Text = execute(); }
            catch (OverflowException) { Error("ผลลัพธ์เกินช่วง Int32 (-2,147,483,648 ถึง 2,147,483,647)"); }
            catch (ArgumentException ex) { Error(ex.Message); }
            catch (FormatException) { Error("กรุณากรอกจำนวนเต็มให้ถูกต้อง"); }
        };
        void Error(string message) { output.ForeColor = Color.Firebrick; output.Text = "ไม่สามารถประมวลผล: " + message; }
    }

    private static int Integer(string input) => int.Parse(input.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);

    // UI convenience only: one item per line, or comma separated. The core accepts actual arrays.
    private static string[] Items(string input) => input.Split(new[] { '\r', '\n', ',' },
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private void BuildBrackets()
    {
        var panel = Page("1. วงเล็บ", "ตรวจชนิดและลำดับวงเล็บ: ( ) [ ] { } เท่านั้น\nข้อความว่าง = true; ช่องว่างหรืออักขระอื่น = false");
        var text = Input(panel, "ข้อความ", "([{}])");
        OutputAndButton(panel, "ตรวจสอบ", () => Algorithms.IsBalanced(text.Text) ? "true" : "false");
    }

    private void BuildSort()
    {
        var panel = Page("2. เรียงข้อความ", "เรียงแบบ Natural Sort ไม่แยกตัวพิมพ์เล็ก/ใหญ่\nค่าที่เท่ากัน เช่น TH2 กับ TH02 คงลำดับเดิม");
        var text = Input(panel, "รายการ (คั่นด้วย comma หรือขึ้นบรรทัดใหม่ ไม่ต้องใส่เครื่องหมายคำพูด)", "TH19\r\nSG20\r\nTH2", true);
        OutputAndButton(panel, "เรียงลำดับ", () => string.Join(Environment.NewLine, Algorithms.NaturalSort(Items(text.Text))));
    }

    private void BuildAutocomplete()
    {
        var panel = Page("3. Autocomplete", "เรียงตรงต้นคำ → กลางคำ → ท้ายคำ; ในกลุ่มเดียวกันคงลำดับเดิม\nคำค้นว่างหรือจำนวนผลลัพธ์ 0 จะคืนรายการว่าง");
        var search = Input(panel, "คำค้น", "th");
        var items = Input(panel, "รายการ (คั่นด้วย comma หรือขึ้นบรรทัดใหม่)", "Mother\r\nThink\r\nWorthy\r\nApple\r\nAndroid", true);
        var max = Input(panel, "จำนวนผลลัพธ์สูงสุด", "2");
        OutputAndButton(panel, "ค้นหา", () =>
        {
            var results = Algorithms.Autocomplete(search.Text, Items(items.Text), Integer(max.Text));
            return results.Length == 0 ? "ไม่พบผลลัพธ์" : string.Join(Environment.NewLine, results);
        });
    }

    private void BuildRoman()
    {
        var panel = Page("4. เลขโรมัน", "รองรับจำนวนเต็ม 1–3999 และเลขโรมันรูปแบบมาตรฐาน เช่น IV, IX, CM");
        var direction = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 710 };
        direction.Items.AddRange(new object[] { "จำนวนเต็ม → เลขโรมัน", "เลขโรมัน → จำนวนเต็ม" });
        direction.SelectedIndex = 0; panel.Controls.Add(direction);
        var text = Input(panel, "ค่าที่ต้องการแปลง", "1989");
        direction.SelectedIndexChanged += (_, _) => text.Text = direction.SelectedIndex == 0 ? "1989" : "MCMLXXXIX";
        OutputAndButton(panel, "แปลงค่า", () => direction.SelectedIndex == 0
            ? Algorithms.ToRoman(Integer(text.Text)) : Algorithms.FromRoman(text.Text).ToString(CultureInfo.InvariantCulture));
    }

    private void BuildDigits()
    {
        var panel = Page("5. เรียงหลักตัวเลข", "รับจำนวนเต็มบวกและเรียงแต่ละหลักจากมากไปน้อย เช่น 3008 → 8300");
        var text = Input(panel, "จำนวนเต็มบวก", "1989");
        OutputAndButton(panel, "เรียงตัวเลข", () => Algorithms.DescendingDigits(Integer(text.Text)).ToString(CultureInfo.InvariantCulture));
    }

    private void BuildTribonacci()
    {
        var panel = Page("6. Tribonacci", "สมาชิกถัดไปคือผลรวม 3 ตัวก่อนหน้า จำนวนผลลัพธ์รวมค่าเริ่มต้นด้วย\nสมมติฐาน: ค่าเริ่มต้นไม่ครบ 3 ตัวให้เติม 0 ทางขวา เช่น [1] → [1,0,0]");
        var seed = Input(panel, "ค่าเริ่มต้น 0–3 ตัว (คั่นด้วย comma; เว้นว่างแทน [])", "1, 3, 5");
        var count = Input(panel, "จำนวนสมาชิก (หน้าจอจำกัดไม่เกิน 10,000 เพื่อความลื่นไหล)", "5");
        OutputAndButton(panel, "สร้างลำดับ", () =>
        {
            int n = Integer(count.Text);
            if (n > 10000) throw new ArgumentOutOfRangeException(nameof(n), "หน้าจอรองรับไม่เกิน 10,000 ตัว");
            return "[" + string.Join(", ", Algorithms.Tribonacci(Items(seed.Text).Select(Integer).ToArray(), n)) + "]";
        });
    }
}
