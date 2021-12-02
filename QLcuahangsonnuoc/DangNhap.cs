using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; //Sử dụng thư viện để làm việc SQL server
using QLcuahangsonnuoc.Class;

namespace QLcuahangsonnuoc
{
    public partial class DangNhap : Form
    {
        string Scon = "Data Source=DESKTOP-5I1LRHO;Initial Catalog=CuaHangSon;Integrated Security=True";
        public DangNhap()
        {
            InitializeComponent();
        }

        private void DangNhap_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if ((txtTaiKhoan.Text == "") && (txtMatKhau.Text == ""))
            {
                MessageBox.Show("Chưa nhập tài khoản!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {     
                    string tk = txtTaiKhoan.Text;
                    string mk = txtMatKhau.Text;
                    string sqlSelect = "select * from TAIKHOAN where TaiKhoan ='" + tk + "' and MatKhau='" + mk + "'";
                    SqlCommand cmd = new SqlCommand(sqlSelect);
                    SqlDataReader dta = cmd.ExecuteReader();
                    if (dta.Read() == true)
                    {
                        this.Hide();
                        Form Mainform = new Mainform();
                        Mainform.Show();
                    }
                    else
                    {
                        MessageBox.Show("Tài Khoản hoặc Mật Khẩu không đúng!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtMatKhau.Text = "";
                    }

            }
        }
    }
}
