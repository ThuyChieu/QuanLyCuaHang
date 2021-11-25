using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using QLcuahangsonnuoc.Class;

namespace QLcuahangsonnuoc
{
    public partial class frmNhap : Form
    {
        DataSet dataSet = new DataSet("CDangIu");

        SqlDataAdapter dapNhap;
        SqlDataAdapter dapNhapChiTiet;

        public frmNhap()
        {
            InitializeComponent();

            dapNhap = Functions.GetDataAdapter("SELECT * FROM Nhap");
            dapNhap.Fill(dataSet, "Nhap");
            dataGridView1.DataSource = dataSet.Tables["Nhap"];

            Functions.FillCombo("SELECT * FROM NhanVien", cmb_MaNV, "MaNV", "Mã Nhân Viên");
            Functions.FillCombo("SELECT * FROM NhaCungCap", cmb_MaNCC, "MaNCC", "Mã Nhà Cung Cấp");

            Functions.FillCombo("SELECT * FROM Hang", cmb_MaHang, "MaHH", "Mã Hàng Hoá");
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txt_MaDNH.Text == "")
            {
                MessageBox.Show("Vui lòng nhập mã Đơn Nhập Hàng", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow newRow = dataSet.Tables["Nhap"].NewRow();
            newRow["MaDNH"] = txt_MaDNH.Text;
            newRow["MaNV"] = cmb_MaNV.Text;
            newRow["NgayNhap"] = dtp_NgayNhap.Value;
            newRow["Tong"] = int.Parse(txt_Tong.Text);

            double tongCong = int.Parse(txt_Tong.Text) * (1 - double.Parse(txt_ChietKhau.Text));
            newRow["TongCong"] = tongCong;
            newRow["MaNCC"] = cmb_MaNCC.Text;
            newRow["ChietKhau"] = double.Parse(txt_ChietKhau.Text);

            dataSet.Tables["Nhap"].Rows.Add(newRow);

            btn_Reset_Click(null, null);
        }

        private void cmb_MaHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            Functions.GetDataAdapter(String.Format("SELECT * FROM Hang WHERE MaHH = '{0}'", cmb_MaHang.Text)).Fill(dataTable);

            if (dataTable.Rows.Count == 0)
                return;
            
            int donGia = (int) dataTable.Rows[0]["DonGiaNhap"];
            txt_DonGia.Text = donGia.ToString();

            DataRow[] foundedNhapChiTiet = dataSet.Tables["Nhap_ChiTiet"].Select(string.Format("MaHH = '{0}' AND MaDNH = '{1}'", dataTable.Rows[0]["MaHH"], txt_MaDNH.Text));

            if (foundedNhapChiTiet.Length == 0)
                txt_SoLuong.Value = 0;
            else
                txt_SoLuong.Value = decimal.Parse(foundedNhapChiTiet[0]["SoLuong"].ToString());
        }

        //Khi thay đổi số lượng thì thực hiện tính lại thành tiền và cập nhật lên bảng Nhap_ChiTiet
        private void txtSoLuong_TextChanged(object sender, EventArgs e)
        {
            int tt, sl, dg;
            sl = (int) txt_SoLuong.Value;
            dg = (int) txt_DonGia.Value;
            tt = sl * dg;
            txt_ThanhTien.Value = tt * (1 - txt_ChietKhau.Value);

            if (sl == 0) return;

            DataRow[] foundedRows = dataSet.Tables["Nhap_ChiTiet"].Select(string.Format("MaHH = '{0}'", cmb_MaHang.Text));

            DataRow updatingRow = null;
            if (foundedRows.Length == 0)
            {
                updatingRow = dataSet.Tables["Nhap_ChiTiet"].NewRow();
                updatingRow["MaDNH"] = txt_MaDNH.Text;
                updatingRow["MaHH"] = cmb_MaHang.Text;

                dataSet.Tables["Nhap_ChiTiet"].Rows.Add(updatingRow);
            }
            else
                updatingRow = foundedRows[0];

            updatingRow["SoLuong"] = sl;
            updatingRow["ThanhTien"] = txt_ThanhTien.Value;

            updateTong();
        }

        private void updateTong()
        {
            txt_Tong.Value = 0;
            foreach (DataRow row in dataSet.Tables["Nhap_ChiTiet"].Rows)
            {
                txt_Tong.Value += (int) row["ThanhTien"];
            }
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            txt_MaDNH.Text = "";
            cmb_MaHang.Text = "";
            txt_SoLuong.Value = 0;
            txt_ThanhTien.Value = 0;
            txt_Tong.Value = 0;
            cmb_MaNCC.Text = "";
            cmb_MaNV.Text = "";
            txt_ChietKhau.Value = 0;
            dtp_NgayNhap.Value = DateTime.Now;
            txt_DonGia.Value = 0;
        }

        private void btn_Xoa_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        private void btn_Luu_Click(object sender, EventArgs e)
        {
            Console.WriteLine(dataSet.Tables["Nhap"].Rows.Count);
            dapNhap.Update(dataSet, "Nhap");
            dapNhapChiTiet.Update(dataSet, "Nhap_ChiTiet");
        }

        private void txt_MaDNH_TextChanged(object sender, EventArgs e)
        {
            if (dataSet.Tables["Nhap_ChiTiet"] != null)
                dataSet.Tables["Nhap_ChiTiet"].Clear();

            dapNhapChiTiet = Functions.GetDataAdapter(string.Format("SELECT * FROM Nhap_ChiTiet WHERE MaDNH = '{0}'", txt_MaDNH.Text));
            dapNhapChiTiet.Fill(dataSet, "Nhap_ChiTiet");
        }

        private void btn_QuayLai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dtp_TimKiemTheoNgay_ValueChanged(object sender, EventArgs e)
        {
            dataSet = new DataSet();
            dapNhap = Functions.GetDataAdapter(string.Format("SELECT * FROM Nhap WHERE DATEDIFF(DAY, NgayNhap, '{0}') = 0", dtp_TimKiemTheoNgay.Value.ToShortDateString()));
            dapNhap.Fill(dataSet, "Nhap");
            dataGridView1.DataSource = dataSet.Tables["Nhap"];
        }

        private void btn_XoaTimKiem_Click(object sender, EventArgs e)
        {
            dtp_TimKiemTheoNgay.Value = DateTime.Now;

            dataSet = new DataSet();
            dapNhap = Functions.GetDataAdapter("SELECT * FROM Nhap");
            dapNhap.Fill(dataSet, "Nhap");
            dataGridView1.DataSource = dataSet.Tables["Nhap"];
        }

        private void btn_Sua_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            txt_MaDNH.Text = row.Cells["MaDNH"].Value.ToString();
            cmb_MaNV.Text = row.Cells["MaNV"].Value.ToString();
            cmb_MaNCC.Text = row.Cells["MaNCC"].Value.ToString();
            dtp_NgayNhap.Value = (DateTime) row.Cells["NgayNhap"].Value;
            txt_ChietKhau.Value = decimal.Parse(row.Cells["ChietKhau"].Value.ToString());
        }
    }
}
