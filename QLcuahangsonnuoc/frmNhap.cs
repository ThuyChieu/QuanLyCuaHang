using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLcuahangsonnuoc.Class;

namespace QLcuahangsonnuoc
{
    public partial class frmNhap : Form
    {

        DataTable dataTable;

        Dictionary<string, decimal> danhSachMaHH = new Dictionary<string, decimal>();

        public frmNhap()
        {
            InitializeComponent();

            dataTable = Functions.GetDataToTable("SELECT * FROM Nhap");
            dataGridView1.DataSource = dataTable;

            Functions.FillCombo("SELECT * FROM NhanVien", cmb_MaNV, "MaNV", "Mã Nhân Viên");
            Functions.FillCombo("SELECT * FROM NhaCungCap", cmb_MaNCC, "MaNCC", "Mã Nhà Cung Cấp");

            Functions.FillCombo("SELECT * FROM Hang", cmb_MaHang, "MaHH", "Mã Hàng Hoá");
            DataTable maHHTable = (DataTable) cmb_MaHang.DataSource;
            foreach (DataRow maHH in maHHTable.Rows)
            {
                danhSachMaHH.Add(maHH["MaHH"].ToString(), 0); // Ban đầu đặt tạm thành tiền của từng hàng hóa = 0
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();
            newRow["MaDNH"] = txt_MaDNH.Text;
            newRow["MaNV"] = cmb_MaNV.Text;
            newRow["NgayNhap"] = dtp_NgayNhap.Value;
            newRow["Tong"] = int.Parse(txt_Tong.Text);

            double tongCong = int.Parse(txt_Tong.Text) * (1 - double.Parse(txt_ChietKhau.Text));
            newRow["TongCong"] = tongCong;
            newRow["MaNCC"] = cmb_MaNCC.Text;
            newRow["ChietKhau"] = double.Parse(txt_ChietKhau.Text);

            dataTable.Rows.Add(newRow);
            dataTable.AcceptChanges();
        }

        private void cmb_MaHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dataTable = Functions.GetDataToTable(String.Format("SELECT DonGiaNhap FROM Hang WHERE MaHH = '{0}'", cmb_MaHang.Text));

            if (dataTable.Rows.Count == 0)
                return;
            
            int donGia = (int) dataTable.Rows[0]["DonGiaNhap"];
            txt_DonGia.Text = donGia.ToString();
        }

        private void txtSoLuong_TextChanged(object sender, EventArgs e)
        {
            //Khi thay đổi số lượng thì thực hiện tính lại thành tiền
            int tt, sl, dg;
            sl = (int) txt_SoLuong.Value;
            dg = (int) txt_DonGia.Value;
            tt = sl * dg;
            txt_ThanhTien.Value = tt * (1 - txt_ChietKhau.Value);

            danhSachMaHH[cmb_MaHang.Text] = txt_ThanhTien.Value;

            updateTong();
        }

        private void updateTong()
        {
            txt_Tong.Value = 0;
            foreach (int value in danhSachMaHH.Values)
            {
                txt_Tong.Value += value;
            }
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            DataTable maHHTable = (DataTable)cmb_MaHang.DataSource;
            foreach (DataRow maHH in maHHTable.Rows)
            {
                danhSachMaHH[maHH["MaHH"].ToString()] = 0; // Ban đầu đặt tạm thành tiền của từng hàng hóa = 0
            }

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
            dataTable.AcceptChanges();
        }
    }
}
