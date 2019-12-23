using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using cf01.CLS;

namespace cf01.MM
{
    public partial class frmMmCalculatePriceFind : Form
    {

        public frmMmCalculatePriceFind()
        {
            InitializeComponent();
        }

        private void frmUnitPriceFormula_Load(object sender, EventArgs e)
        {
            dgvDetails.AutoGenerateColumns = false;

        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void findData()
        {
            DataTable dtId = clsMmCalculatePrice.findMmCalculatePrice(txtId.Text,txtMo_id.Text,txtMat_type.Text,txtPrd_Type.Text,txtArt.Text,txtSize.Text
                ,txtFid.Text,txtCrusr.Text,txtCrtim_from.Text,txtCrtim_to.Text,txtCdesc_D.Text,txtCdesc.Text);
            dgvDetails.DataSource = dtId;
            if (dgvDetails.Rows.Count > 0)
            {
                string id = dgvDetails.Rows[0].Cells["colId"].Value.ToString().Trim();
                string ver = dgvDetails.Rows[0].Cells["colVer"].Value.ToString().Trim();
                Color cl1 = Color.White;
                Color cl2 = Color.FromArgb(0xCC, 0xFF, 0xFF);
                Color cl3 = Color.White;
                int j = 1;
                int k = 0;
                for (int i = 0; i < dgvDetails.Rows.Count; i++)
                {
                    //dgvDetails.Rows[i].DefaultCellStyle.BackColor = cl3;
                    if (dgvDetails.Rows[i].Cells["colId"].Value.ToString().Trim() == id && dgvDetails.Rows[i].Cells["colVer"].Value.ToString().Trim() == ver)
                        //dgvDetails.Rows[i].DefaultCellStyle.BackColor = cl3;
                        k = 1;
                    else
                    {
                        //dgvDetails.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(0xCC, 0xFF, 0xFF);
                        id = dgvDetails.Rows[i].Cells["colId"].Value.ToString().Trim();
                        ver = dgvDetails.Rows[i].Cells["colVer"].Value.ToString().Trim();
                        j = j + 1;
                        //cl3 = Color.FromArgb(0xCC, 0xFF, 0xFF);
                    }
                    if (j % 2 == 1)
                    {
                        dgvDetails.Rows[i].DefaultCellStyle.BackColor = cl1;
                    }
                    else
                        dgvDetails.Rows[i].DefaultCellStyle.BackColor = cl2;
                }
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            findData();
        }

        private void dgvDetails_DoubleClick(object sender, EventArgs e)
        {
            frmMmCalculatePrice.get_id = dgvDetails.Rows[dgvDetails.CurrentCell.RowIndex].Cells["colId"].Value.ToString();
            frmMmCalculatePrice.get_ver = Convert.ToInt32(dgvDetails.Rows[dgvDetails.CurrentCell.RowIndex].Cells["colVer"].Value.ToString());
            this.Close();
        }

        private void txtMat_type_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
        }

    }
}
