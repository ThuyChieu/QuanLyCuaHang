﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLcuahangsonnuoc
{
    public partial class Mainform : Form
    {
        public Mainform()
        {
            InitializeComponent();
        }

        private void hoaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void hóaĐơnNhậpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new frmNhap()).ShowDialog();
        }

        private void Mainform_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
