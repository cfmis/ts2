using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using cf01.CLS;
using cf01.Forms;
using System.Threading;

namespace cf01.MM
{
    public partial class frmProductCostingFindPrice : Form
    {
        public frmProductCostingFindPrice()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmProductCostingFindPrice_Load(object sender, EventArgs e)
        {
            dgvDetails1.AutoGenerateColumns = false;
            dgvDetails2.AutoGenerateColumns = false;
            txtDepId.Text = frmProductCosting.searchDepId;
            txtMaterialId.Text = frmProductCosting.searchProductId;
            txtMaterialName.Text = frmProductCosting.searchProductName;
            if (txtDepId.Text == "501" || txtDepId.Text == "510")
                xtcFind.SelectedTabPageIndex = 1;
            //selectFind();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            

            frmProgress wForm = new frmProgress();
            new Thread((ThreadStart)delegate
            {
                wForm.TopMost = true;
                wForm.ShowDialog();
            }).Start();

            //**********************
            selectFind(); //数据处理

            //genBomTree(pid);
            //**********************
            wForm.Invoke((EventHandler)delegate { wForm.Close(); });
        }
        private void selectFind()
        {
            if (xtcFind.SelectedTabPageIndex == 0)
                findMaterialPrice();
            else if (xtcFind.SelectedTabPageIndex == 1)
                findPlatePrice();
        }
        private void findMaterialPrice()
        {
            DataTable dtMaterialPrice = clsProductCosting.findMaterialPrice(txtMaterialId.Text,txtMaterialName.Text);
            dgvDetails1.DataSource = dtMaterialPrice;
            if (dgvDetails1.Rows.Count == 0)
                MessageBox.Show("沒有找到符合條件的記錄!");
        }
        private void findPlatePrice()
        {
            DataTable dtPlatePrice = clsProductCosting.findPlatePrice(txtDepId.Text,txtMaterialId.Text, txtMaterialName.Text);
            dgvDetails2.DataSource = dtPlatePrice;
            if (dgvDetails2.Rows.Count == 0)
                MessageBox.Show("沒有找到符合條件的記錄!");
        }
        private void dgvDetails1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            frmProductCosting.searchPrice = dgvDetails1.Rows[dgvDetails1.CurrentRow.Index].Cells["colMaterialPriceHkd"].Value.ToString() != ""
                ? Convert.ToDecimal(dgvDetails1.Rows[dgvDetails1.CurrentRow.Index].Cells["colMaterialPriceHkd"].Value) : 0;
            this.Close();
        }

        private void dgvDetails2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            frmProductCosting.searchPrice = dgvDetails2.Rows[dgvDetails2.CurrentRow.Index].Cells["colPlatePriceKg"].Value.ToString() != ""
                ? Convert.ToDecimal(dgvDetails2.Rows[dgvDetails2.CurrentRow.Index].Cells["colPlatePriceKg"].Value) : 0;
            this.Close();
        }
    }
}
