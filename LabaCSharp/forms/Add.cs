using System;
using System.Windows.Forms;
using LabaCSharp.Generators.Types;

namespace LabaCSharp.Forms
{
    public partial class Add : Form
    {
        public string GeneratorName => name.Text;

        public GeneratorType Type { get; private set; }

        public int Count { get; private set; }

        public int FirstNumber { get; private set; }

        public int Step { get; private set; }

        public Add()
        {
            InitializeComponent();
        }

        private void type_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.step.Enabled = this.type.SelectedIndex == (int)GeneratorType.STEP;
            this.firstNumber.Enabled = this.type.SelectedIndex == (int)GeneratorType.STEP;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Type = (GeneratorType)this.type.SelectedIndex;

            try
            {
                this.Count = Convert.ToInt32(this.N.Text);
                if (Count <= 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Неверно указано количество чисел!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.Type == GeneratorType.STEP)
            {
                try
                {
                    this.FirstNumber = Convert.ToInt32(this.firstNumber.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Неверно указано начальное число!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    this.Step = Convert.ToInt32(this.step.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Неверно указан шаг!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
