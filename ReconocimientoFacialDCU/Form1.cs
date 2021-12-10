using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace ReconocimientoFacialDCU
{
    public partial class Form1 : Form
    {

        // Variables vectores y Harcascades
        int con = 0;
        Image<Bgr, Byte> currentFrame;
        Capture Grabar;
        HaarCascade face;//Rostro
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.8d, 0.8d);
        Image<Gray, byte> result, TraineFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> labels1 = new List<string>();
        List<string> labels2 = new List<string>();
        List<string> NombrePersonas = new List<string>();
        List<string> TelefonoPersona = new List<string>();
        List<string> CorreoPersona = new List<string>();
        int ContTrain, numLabels, t;
        string Nombre;
        string Telefono;
        string Correo;
        DataGridView d = new DataGridView();

        public Form1()
        {
            InitializeComponent();

            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                clsDA.Consultar(d);

                string[] Labels = clsDA.Nombre;
                string[] Labels1 = clsDA.Telefono;
                string[] Labels2 = clsDA.Correo;
                numLabels = clsDA.TotalRostros;
                ContTrain = numLabels;

                for (int i = 0; i < numLabels; i++)
                {
                    con = i;
                    Bitmap bmp = new Bitmap(clsDA.ConvertBinaryToImg(con));

                    trainingImages.Add(new Image<Gray, byte>(bmp));
                    labels.Add(Labels[i]);
                    labels1.Add(Labels1[i]);
                    labels2.Add(Labels2[i]);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Base de sin Rostros para cargar");
            }
        }

        private void FrameGrabar(object sender, EventArgs e)
        {
            lblCantidad.Text = "0";
            NombrePersonas.Add("");
            TelefonoPersona.Add("");
            CorreoPersona.Add("");

            lblNombre.Text = "";
            lblTelefono.Text = "";
            lblCorreo.Text = "";

            try
            {
                currentFrame = Grabar.QueryFrame().Resize(720, 480, INTER.CV_INTER_CUBIC);
                gray = currentFrame.Convert<Gray, Byte>();
                MCvAvgComp[][] RostrosDetectados = gray.DetectHaarCascade(face, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                foreach(MCvAvgComp R in RostrosDetectados[0]){
                    t = t + 1;
                    result = currentFrame.Copy(R.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    currentFrame.Draw(R.rect, new Bgr(Color.Green), 2);

                    if (trainingImages.ToArray().Length != 0)
                    {
                        MCvTermCriteria Criterio = new MCvTermCriteria(ContTrain, 0.88);
                        MCvTermCriteria Criterio1 = new MCvTermCriteria(ContTrain, 0.88);
                        MCvTermCriteria Criterio2 = new MCvTermCriteria(ContTrain, 0.88);
                        EigenObjectRecognizer recogida = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(), ref Criterio);
                        EigenObjectRecognizer recogida1 = new EigenObjectRecognizer(trainingImages.ToArray(), labels1.ToArray(), ref Criterio1);
                        EigenObjectRecognizer recogida2 = new EigenObjectRecognizer(trainingImages.ToArray(), labels2.ToArray(), ref Criterio2);
                        var fa = new Image<Gray, byte>[trainingImages.Count];
                        
                        Nombre = recogida.Recognize(result);
                        Telefono = recogida1.Recognize(result);
                        Correo = recogida2.Recognize(result);
                        currentFrame.Draw(Nombre, ref font, new Point(R.rect.X, R.rect.Y), new Bgr(Color.Red));
                    }
                    NombrePersonas[t - 1] = Nombre;
                    NombrePersonas.Add("");

                    lblCantidad.Text = RostrosDetectados[0].Length.ToString();
                    lblNombre.Text = Nombre;
                    lblTelefono.Text = Telefono;
                    lblCorreo.Text = Correo;
                }
                t = 0;
                imageBox1.Image = currentFrame;
                Nombre = "";
                NombrePersonas.Clear();
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            reconocer();
        }

        private void reconocer()
        {
            try
            {
                Grabar = new Capture();
                Grabar.QueryFrame();
                Application.Idle += new EventHandler(FrameGrabar);
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message, "Error");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void DetenerReconocer()
        {
            try
            {
                Application.Idle -= new EventHandler(FrameGrabar);
                Grabar.Dispose();
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message, "Error");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmRgistrar frmRgistrar = new frmRgistrar();

            DetenerReconocer();
            this.Hide();
            frmRgistrar.ShowDialog();
        }
    }
}
