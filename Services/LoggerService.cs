using System;
using System.Drawing;
using System.Windows.Forms;

namespace OVRefiner.Services
{
    public class LoggerService
    {
        private readonly RichTextBox _logBox;

        public LoggerService(RichTextBox logBox)
        {
            _logBox = logBox;
        }

        public void LogInfo(string message) => AppendText($"[INFO] {message}", Color.White);
        public void LogSuccess(string message) => AppendText($"[SUCCESS] {message}", Color.LightGreen);
        public void LogWarning(string message) => AppendText($"[WARN] {message}", Color.Orange);
        public void LogError(string message) => AppendText($"[ERROR] {message}", Color.Red);

        private void AppendText(string text, Color color)
        {
            if (_logBox.InvokeRequired)
            {
                _logBox.Invoke(new Action(() => AppendText(text, color)));
                return;
            }

            _logBox.SelectionStart = _logBox.TextLength;
            _logBox.SelectionLength = 0;
            _logBox.SelectionColor = color;
            _logBox.AppendText($"{DateTime.Now:HH:mm:ss} - {text}{Environment.NewLine}");
            _logBox.SelectionColor = _logBox.ForeColor;
            _logBox.ScrollToCaret();
        }
    }
}