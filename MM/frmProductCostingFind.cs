using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using cf01.CLS;
using cf01.Forms;
using cf01.Reports;
using DevExpress.XtraReports.UI;

namespace cf01.MM
{
    public partial class frmProductCostingFind : Form
    {
        private DataTable dtWipData = new DataTable();
        private DataTable dtProductCosting = new DataTable();
        public frmProductCostingFind()
        {
            InitializeComponent();
        }
        private void frmMmCostingFind_Load(object sender, EventArgs e)
        {
            dgvCosting.AutoGenerateColumns = false;
            dgvWipData.AutoGenerateColumns = false;
            rdgIsSetCosting.SelectedIndex = 2;
            //txtDateFrom.Text = System.DateTime.Now.AddDays(-90).ToString("yyyy/MM/dd");
            //txtDateTo.Text = System.DateTime.Now.ToString("yyyy/MM/dd");
            txtProductMo.Focus();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            frmProductCosting.searchProductId = "";
            frmProductCosting.searchProductName = "";
            frmProductCosting.searchProductMo = "";
            this.Close();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            txtProductMo.Focus();
            if (rdgIsSetCosting.SelectedIndex != 0)
            {
                if (txtMatFrom.Text.Trim() == ""
                    && txtPrdTypeFrom.Text.Trim() == ""
                    && txtArtFrom.Text.Trim() == ""
                    && txtSizeFrom.Text.Trim() == ""
                    && txtClrFrom.Text.Trim() == ""
                    && txtProductMo.Text.Trim() == ""
                    && txtDateFrom.Text.Trim()==""
                    && txtDateTo.Text.Trim()=="" )
                {
                    MessageBox.Show("請輸入查詢條件!");
                    return;
                }
            }
            frmProgress wForm = new frmProgress();
            new Thread((ThreadStart)delegate
            {
                wForm.TopMost = true;
                wForm.ShowDialog();
            }).Start();

            //**********************
            findProcess(); //数据处理

            //genBomTree(pid);
            //**********************
            wForm.Invoke((EventHandler)delegate { wForm.Close(); });

            
        }
        private void findProcess()
        {
            int isSetFlag = rdgIsSetCosting.SelectedIndex;
            dtProductCosting = clsProductCosting.findProductCosting(isSetFlag, rdgSource.SelectedIndex, chkShowF0.Checked, txtProductMo.Text.Trim()
                ,txtMatFrom.Text.Trim(),txtMatTo.Text.Trim(),txtPrdTypeFrom.Text.Trim(),txtPrdTypeTo.Text.Trim()
                ,txtArtFrom.Text.Trim(),txtArtTo.Text.Trim(),txtSizeFrom.Text.Trim(),txtSizeTo.Text.Trim()
                ,txtClrFrom.Text.Trim(),txtClrTo.Text.Trim(),txtDateFrom.Text.Trim(),txtDateTo.Text.Trim()
                ,txtMoGroup.Text.Trim(),txtSales.Text.Trim()
                );
            dgvCosting.DataSource = dtProductCosting;
            if (dgvCosting.Rows.Count == 0)
                MessageBox.Show("沒有找到符合條件的記錄");
            else
            {
                for (int i = 0; i < dgvCosting.Rows.Count; i++)
                {
                    dgvCosting.Rows[i].Cells["colSetCosting"].Value = "...";
                }
            }
        }
        
        private void txtMatFrom_Leave(object sender, EventArgs e)
        {
            txtMatTo.Text = txtMatFrom.Text;
        }

        private void txtPrdTypeFrom_Leave(object sender, EventArgs e)
        {
            txtPrdTypeTo.Text = txtPrdTypeFrom.Text;
        }

        private void txtSizeFrom_Leave(object sender, EventArgs e)
        {
            txtSizeTo.Text = txtSizeFrom.Text;
        }

        private void txtClrFrom_Leave(object sender, EventArgs e)
        {
            txtClrTo.Text = txtClrFrom.Text;
        }

        private void txtArtFrom_Leave(object sender, EventArgs e)
        {
            txtArtTo.Text = txtArtFrom.Text;
        }

        private void txtDateFrom_Leave(object sender, EventArgs e)
        {
            txtDateTo.Text = txtDateFrom.Text;
        }

        private void dgvCosting_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCosting.Rows.Count == 0)
                return;
            string productMo = dgvCosting.Rows[dgvCosting.CurrentRow.Index].Cells["colProductMo"].Value == null ? "" : dgvCosting.Rows[dgvCosting.CurrentRow.Index].Cells["colProductMo"].Value.ToString();
            dtWipData = clsProductCosting.getWipData(productMo);
            dgvWipData.DataSource = dtWipData;
            for (int i = 0; i < dgvWipData.Rows.Count; i++)
            {
                dgvWipData.Rows[i].Cells["colWipSetCosting"].Value = "...";
            }
        }

        private void dgvWipData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvWipData.Columns[e.ColumnIndex].Name == "colWipSetCosting")
            {
                DataGridViewRow dgr = dgvWipData.Rows[dgvWipData.CurrentRow.Index];
                frmProductCosting.searchProductId = dgr.Cells["colWipGoodsId"].Value.ToString();
                frmProductCosting.searchProductName = dgr.Cells["colWipGoodsCname"].Value.ToString();
                frmProductCosting.searchProductMo = dgr.Cells["colWipProductMo"].Value.ToString();
                this.Close();
            }
        }

        private void dgvCosting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCosting.Columns[e.ColumnIndex].Name == "colSetCosting")
            {
                DataGridViewRow dgr = dgvCosting.Rows[dgvWipData.CurrentRow.Index];
                frmProductCosting.searchProductId = dgr.Cells["colProductId"].Value.ToString();
                frmProductCosting.searchProductName = dgr.Cells["colProductName"].Value.ToString();
                frmProductCosting.searchProductMo = dgr.Cells["colProductMo"].Value.ToString();
                this.Close();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printData(dtProductCosting);
        }
        private void printData(DataTable dt)
        {
            xrProductCostingFind oRepot = new xrProductCostingFind() { DataSource = dt};
            oRepot.CreateDocument();
            oRepot.PrintingSystem.ShowMarginsWarning = false;
            oRepot.ShowPreview();
        }

        private void btnPrintWipData_Click(object sender, EventArgs e)
        {
            printData(dtWipData);
        }

    }
}
