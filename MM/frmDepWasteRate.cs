using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using cf01.MDL;
using cf01.CLS;

namespace cf01.MM
{
    public partial class frmDepWasteRate : Form
    {
        public frmDepWasteRate()
        {
            InitializeComponent();
        }

        private void frmDepWasteRate_Load(object sender, EventArgs e)
        {
            initData();
        }
        private void initData()
        {
            lueDepId.Properties.ValueMember = "dep_id"; //相当于Editvalue
            lueDepId.Properties.DisplayMember = "dep_cdesc"; //相当于Text
            lueDepId.Properties.DataSource = clsDepWasteRate.loadDep();
            binddDepWasteRate();
        }
        private void binddDepWasteRate()
        {
            dgvDetails.DataSource = clsDepWasteRate.loadDepWasteRate();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvDetails_SelectionChanged(object sender, EventArgs e)
        {
            fillTextBox(dgvDetails.CurrentRow.Index);
        }
        private void fillTextBox(int row)
        {
            DataGridViewRow dgr = dgvDetails.Rows[row];
            lueDepId.EditValue = dgr.Cells["colDepId"].Value.ToString();
            txtWasteRate.Text = dgr.Cells["colWasteRate"].Value.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            save();
        }
        private void save()
        {
            string result = "";
            decimal wasteRate=txtWasteRate.Text!=""?Convert.ToDecimal(txtWasteRate.Text):0;
            result = clsDepWasteRate.updateDepWasteRate(lueDepId.EditValue.ToString(), wasteRate);
            if (result == "")
            {
                MessageBox.Show("儲存成功!");
                binddDepWasteRate();
            }
            else
                MessageBox.Show("儲存失敗!");
        }
    }
}
