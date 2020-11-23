using System;
using System.Windows.Forms;
using LabaCSharp.Generators;

namespace LabaCSharp.Forms
{

    public partial class Main : Form
    {
        private BaseGenerator root = new BaseGenerator("__BASE__", 1);

        public Main()
        {
            InitializeComponent();
            this.load.Click += this.OpenFile;
            this.save.Click += this.SaveFile;
        }

        private void AddToTree(BaseGenerator generator, TreeNode node = null)
        {
            TreeNode treeNode;

            if (node == null)
            {
                treeNode = treeView.Nodes.Add(generator.Name); 
            } else
            {
                treeNode = node.Nodes.Add(generator.Name);
            }

            if (generator.Generators.Count != 0)
            {
                foreach (BaseGenerator child in generator.Generators)
                {
                    this.AddToTree(child, treeNode);
                }
            }
        }

        private bool AddToGenerator(string target, BaseGenerator child, BaseGenerator parent = null)
        {
            BaseGenerator _parent;

            if (parent == null)
            {
                _parent = root;
            }
            else
            {
                _parent = parent;
            }

            for (int i = 0; i < _parent.Generators.Count; i++)
            {
                BaseGenerator generator = (BaseGenerator)_parent.Generators[i];

                if (generator.Name == target)
                {
                    generator.Add(child);
                    return true;
                }

                if (AddToGenerator(target, child, generator))
                {
                    return true;
                }
            }

            return false;
        }

        private BaseGenerator SearchGenerator(string target, BaseGenerator parent = null)
        {
            BaseGenerator _parent;

            if (parent == null)
            {
                _parent = root;
            }
            else
            {
                _parent = parent;
            }

            for (int i = 0; i < _parent.Generators.Count; i++)
            {
                BaseGenerator _gen, generator = (BaseGenerator)_parent.Generators[i];

                if (generator.Name == target)
                {
                    return generator;
                }

                _gen = SearchGenerator(target, generator);

                if (_gen != null)
                {
                    return _gen;
                }
            }

            return null;
        }

        private void add_Click(object sender, EventArgs e)
        {
            BaseGenerator generator;
            
            using (var form = new Add())
            {

                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (SearchGenerator(form.GeneratorName) != null)
                {
                    MessageBox.Show("Генератор с таким названием уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                switch (form.Type)
                {
                    case Generators.Types.GeneratorType.BASE:
                        generator = new BaseGenerator(form.GeneratorName, form.Count);
                        break;
                    case Generators.Types.GeneratorType.RAND:
                        generator = new RandomGenerator(form.GeneratorName, form.Count);
                        break;
                    case Generators.Types.GeneratorType.STEP:
                        generator = new GeneratorWithStep(form.GeneratorName, form.Count, form.FirstNumber, form.Step);
                        break;
                    default:
                        return;
                }
            }

            if (this.treeView.SelectedNode == null)
            {
                this.root.Add(generator);
                AddToTree(generator);

                return;
            }

            BaseGenerator parent = SearchGenerator(this.treeView.SelectedNode.Text);
            parent.Add(generator);

            this.treeView.BeginUpdate();
            AddToTree(generator, this.treeView.SelectedNode);
            this.treeView.EndUpdate();
        }

        private bool DeleteGenerator(string target, BaseGenerator parent = null)
        {
            BaseGenerator _parent;

            if (parent == null)
            {
                _parent = root;
            }
            else
            {
                _parent = parent;
            }

            for (int i = 0; i < _parent.Generators.Count; i++)
            {
                BaseGenerator generator = (BaseGenerator)_parent.Generators[i];

                if (generator.Name == target)
                {
                    _parent.Generators.RemoveAt(i);
                    return true;
                }
                
                if (DeleteGenerator(target, generator))
                {
                    return true;
                }
            }

            return false;
        }

        private bool DeleteTreeNode(string target, TreeNode parent = null)
        {
            TreeNodeCollection children;
            
            if (parent == null)
            {
                children = this.treeView.Nodes;
            }
            else
            {
                children = parent.Nodes;
            }

            foreach (TreeNode child in children)
            {
                if (child.Text == target)
                {
                    child.Remove();
                    return true;
                }

                if (DeleteTreeNode(target, child))
                {
                    return true;
                }
            }

            return false;
        }

        private void generate_Click(object sender, EventArgs e)
        {
            if (this.root.Generators.Count == 0)
            {
                MessageBox.Show("Нет генераторов.\nПожалуйста, добавьте один генератор и попробуйте ещё раз.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                MessageBox.Show($"Сгенерированное число - {root.Generate()}", "Генерация числа");
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при генерировании числа!\nПожайлуйста, проверьте, что все базовые генераторы содержат другие генераторы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView.SelectedNode;

            if (node == null)
            {
                MessageBox.Show("Не выбран элемент, который нужно удалить!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DeleteGenerator(node.Text);
            this.treeView.BeginUpdate();
            DeleteTreeNode(node.Text);
            this.treeView.EndUpdate();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            if (this.openFile.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (var fs = this.openFile.OpenFile())
            {
                this.root = BaseGenerator.Load(fs);
                this.treeView.BeginUpdate();
                this.treeView.Nodes.Clear();
                for (int i = 0; i < this.root.Generators.Count; i++)
                {
                    BaseGenerator generator = (BaseGenerator)this.root.Generators[i];
                    AddToTree(generator);
                }
                this.treeView.EndUpdate();
            }
        }

        private void SaveFile(object sender, EventArgs e)
        {
            if (this.saveFile.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (var fs = this.saveFile.OpenFile())
            {
                this.root.Save(fs);
            }
        }
    }
}
