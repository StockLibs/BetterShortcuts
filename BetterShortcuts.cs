using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;

public class ArrowPatcherForm : Form {
    private Label titleLabel;
    private Label subtitleLabel;
    private Panel mainPanel;
    private Button btnHideArrowsImageres;
    private Button btnHideArrows50;
    private Button btnCustomArrow;
    private Button btnRestoreDefault;
    private Label statusLabel;

    [STAThread]
    public static void Main() {
        if (!IsRunningAsAdmin()) {
            ElevateToAdmin();
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ArrowPatcherForm());
    }

    private static bool IsRunningAsAdmin() {
        try {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        } catch {
            return false;
        }
    }

    private static void ElevateToAdmin() {
        ProcessStartInfo proc = new ProcessStartInfo();
        proc.UseShellExecute = true;
        proc.WorkingDirectory = Environment.CurrentDirectory;
        proc.FileName = Application.ExecutablePath;
        proc.Verb = "runas";

        try {
            Process.Start(proc);
        } catch {
            MessageBox.Show("This application requires Administrator privileges to edit the registry.", "Elevation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    public ArrowPatcherForm() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        this.Text = "BetterShortcuts";
        this.Size = new Size(550, 420);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        this.BackColor = Color.FromArgb(243, 243, 243); 
        this.ForeColor = Color.FromArgb(32, 32, 32);

        titleLabel = new Label {
            Text = "Shortcut Arrow Manager",
            Font = new Font("Segoe UI Semibold", 18, FontStyle.Regular),
            Location = new Point(30, 25),
            Size = new Size(490, 35),
            ForeColor = Color.FromArgb(0, 102, 204)
        };

        subtitleLabel = new Label {
            Text = "Customize or hide the shortcut overlay arrow on your desktop icons.",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            Location = new Point(30, 60),
            Size = new Size(490, 20),
            ForeColor = Color.FromArgb(100, 100, 100)
        };

        mainPanel = new Panel {
            Location = new Point(30, 95),
            Size = new Size(475, 230),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Button 1: The Golden Method (imageres.dll,197)
        btnHideArrowsImageres = CreateWindowsButton("Hide Arrows (Recommended - imageres.dll)", 25, 20, 425, 40);
        btnHideArrowsImageres.Click += (s, e) => {
            SetRegistryValue(@"%windir%\System32\imageres.dll,197");
            statusLabel.Text = "Applied: Hidden via imageres.dll,197! Explorer restarted.";
        };

        // Button 2: Legacy Hide (-50)
        btnHideArrows50 = CreateWindowsButton("Hide Arrows (Fallback - shell32.dll,-50)", 25, 70, 425, 40);
        btnHideArrows50.Click += (s, e) => {
            SetRegistryValue(@"%windir%\System32\shell32.dll,-50");
            statusLabel.Text = "Applied: Hidden via shell32.dll,-50. Explorer restarted!";
        };

        // Button 3: Custom Icon (.ico File)
        btnCustomArrow = CreateWindowsButton("Select Custom Icon (.ico File Only)...", 25, 120, 425, 40);
        btnCustomArrow.Click += (s, e) => {
            HandleCustomImageSelection();
        };

        // Button 4: Restore Windows Default
        btnRestoreDefault = CreateWindowsButton("Restore Windows Default Arrows", 25, 170, 425, 40);
        btnRestoreDefault.Click += (s, e) => {
            DeleteRegistryValue();
            statusLabel.Text = "Restored: Standard shortcut arrows. Explorer restarted!";
        };

        mainPanel.Controls.Add(btnHideArrowsImageres);
        mainPanel.Controls.Add(btnHideArrows50);
        mainPanel.Controls.Add(btnCustomArrow);
        mainPanel.Controls.Add(btnRestoreDefault);

        statusLabel = new Label {
            Text = "Ready. Run with admin privileges.",
            Font = new Font("Segoe UI Italic", 9, FontStyle.Regular),
            Location = new Point(30, 340),
            Size = new Size(475, 25),
            ForeColor = Color.FromArgb(80, 80, 80),
            TextAlign = ContentAlignment.MiddleLeft
        };

        this.Controls.Add(titleLabel);
        this.Controls.Add(subtitleLabel);
        this.Controls.Add(mainPanel);
        this.Controls.Add(statusLabel);
    }

    private Button CreateWindowsButton(string text, int x, int y, int width, int height) {
        Button btn = new Button {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, height),
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            BackColor = Color.FromArgb(251, 251, 251),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderColor = Color.FromArgb(204, 204, 204);
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(229, 241, 251); 
        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(204, 228, 247);
        return btn;
    }

    private void SetRegistryValue(string value) {
        try {
            using (RegistryKey explorerKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", true)) {
                if (explorerKey != null) {
                    using (RegistryKey shellIconsKey = explorerKey.CreateSubKey("Shell Icons")) {
                        shellIconsKey.SetValue("29", value, RegistryValueKind.String);
                    }
                }
            }
            RestartExplorer();
        } catch (Exception ex) {
            MessageBox.Show("Registry write failed: " + ex.Message);
        }
    }

    private void DeleteRegistryValue() {
        try {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons", true)) {
                if (key != null) {
                    key.DeleteValue("29", false);
                }
            }
            RestartExplorer();
        } catch (Exception ex) {
            MessageBox.Show("Registry delete failed: " + ex.Message);
        }
    }

    private void HandleCustomImageSelection() {
        using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
            openFileDialog.Title = "Select Custom Icon";
            openFileDialog.Filter = "Icon Files (*.ico)|*.ico";

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                string selectedPath = openFileDialog.FileName;
                string targetPath = @"C:\Windows\custom.ico";

                try {
                    File.Copy(selectedPath, targetPath, true);
                    SetRegistryValue(targetPath);
                    statusLabel.Text = "Success: Custom icon copied and applied!";
                } catch (Exception ex) {
                    MessageBox.Show("Failed to copy file.\n\nError: " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void RestartExplorer() {
        try {
            foreach (Process p in Process.GetProcessesByName("explorer")) {
                p.Kill();
                p.WaitForExit();
            }
            Process.Start("explorer.exe");
        } catch {}
    }
}
