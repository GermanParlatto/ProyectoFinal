﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Dominio;
using System.Drawing;

namespace UI
{
    public partial class Form_FuentesRSS : Form
    {
        #region Variables
        /// <summary>
        /// Lista de FuenteRSS a agregar
        /// </summary>
        private List<FuenteRSS> iListaFuenteRSSAgregar;
        /// <summary>
        /// Lista de FuenteRSS a actualizar
        /// </summary>
        private List<FuenteRSS> iListaFuenteRSSActualizar;
        /// <summary>
        /// Lista de FuenteRSS a eliminar
        /// </summary>
        private List<FuenteRSS> iListaFuenteRSSEliminar;
        /// <summary>
        /// Lista de FuenteRSS a mostrar
        /// </summary>
        private List<FuenteRSS> iListaFuenteRSS;
        /// <summary>
        /// Valor de la fuente de RSS
        /// </summary>
        private string iValorRSS;
        /// <summary>
        /// Variable booleana que determina si es necesario seleccionar un RSS para salir de la ventana
        /// </summary>
        private bool iNecesitaSeleccionar;
        #endregion

        #region Región: Inicialización y Carga
        /// <summary>
        /// Constructor de la ventana
        /// </summary>
        /// <param name="pNecesitaseleccionar">Variable booleana que determina si se necesita seleccionar un RSS</param>
        /// <param name="pFuente">Fuentes RSS actual</param>
        internal Form_FuentesRSS(bool pNecesitaseleccionar = false)
        {
            InitializeComponent();
            this.iListaFuenteRSS = new List<FuenteRSS>();
            this.iListaFuenteRSSActualizar = new List<FuenteRSS>();
            this.iListaFuenteRSSEliminar = new List<FuenteRSS>();
            this.iListaFuenteRSSAgregar = new List<FuenteRSS>();
            this.backgroundWorker_ObtenerRSS.RunWorkerAsync();
            this.AcceptButton = this.button_Aceptar;
            this.CancelButton = this.button_Volver;
            this.iNecesitaSeleccionar = pNecesitaseleccionar;
            this.tableLayoutPanel1.Visible = false;
            this.CampoCompleto(this.pictureBox_ComprobacionDescripcion, false);
            this.CampoCompleto(this.pictureBox_ComprobacionURL, false);
            this.VisualizarEdicionRSS(false);
        }
        #endregion

        #region Región: Métodos Extra Comunes
        /// <summary>
        /// Devuelve la Fuente Seleccionada
        /// </summary>
        /// <returns>Tipo de dato FuenteRSS que representa la fuente seleccionada</returns>
        private FuenteRSS FuenteSeleccionada()
        {
            return (FuenteRSS)this.dataGridView.CurrentRow.DataBoundItem;
        }

        /// <summary>
        /// Actualiza el DataGridView
        /// </summary>
        private void ActualizarDGV()
        {
            //this.dataGridView.DataSource = typeof(List<FuenteRSS>);
            //this.dataGridView.DataSource = this.iListaFuenteRSS;
            (this.dataGridView.BindingContext [this.dataGridView.DataSource] as CurrencyManager).Refresh();
            this.dataGridView.Update();
            this.dataGridView.Refresh();
        }

        /// <summary>
        /// Activa el botón Aceptar si todos los Campos están llenos
        /// </summary>
        private void ActivarAceptar()
        {
            this.button_Aceptar.Enabled = ((this.iListaFuenteRSS.Count > 0) || (this.iListaFuenteRSSActualizar.Count > 0) ||
                                          (this.iListaFuenteRSSAgregar.Count > 0) || (this.iListaFuenteRSSEliminar.Count > 0)) &&
                                          ((!this.iNecesitaSeleccionar) || (this.dataGridView.RowCount > 0));
        }

        /// <summary>
        /// Determina el ícono que representa el estado del campo
        /// </summary>
        /// <param name="pPictureBox">Form que contiene la imagen</param>
        /// <param name="value">Valor booleano que representa si está o no completo el campo correspondiente</param>
        private void CampoCompleto(PictureBox pPictureBox, bool value)
        {
            int anchoComun = pPictureBox.Width;
            int altoComun = pPictureBox.Height;
            if (value)
            {
                pPictureBox.Image = ImagenServices.CambiarTamañoImagen(Properties.Resources.greenTick, anchoComun, altoComun);

            }
            else
            {
                pPictureBox.Image = ImagenServices.CambiarTamañoImagen(SystemIcons.Exclamation.ToBitmap(), anchoComun, altoComun);
            }
        }

        /// <summary>
        /// Habilita los botones de edición a partir del valor
        /// </summary>
        /// <param name="value">Valor de habilitación</param>
        private void HabilitarBotones(bool value)
        {
            this.button_Cancelar.Visible = !value;
            this.tableLayoutPanel1.Visible = !value;
            this.button_Nuevo.Visible = value;
            this.button_Modificar.Visible = value;
            this.button_Eliminar.Visible = value;
            this.button_Modificar.Enabled = value;
            this.button_Eliminar.Enabled = value;
            this.button_Aceptar.Visible = value;
            this.dataGridView.Enabled = value;
            this.button_AceptarNuevo.Visible = false;
            this.button_AceptarModificar.Visible = false;
            this.button_AceptarNuevo.Enabled = false;
            this.button_AceptarModificar.Enabled = false;
        }

        /// <summary>
        /// Activa la visualización del valor de la fuente RSS
        /// </summary>
        /// <param name="value">Valor de visualización o no de la fuente RSS</param>
        private void VisualizarEdicionRSS(bool value)
        {
            this.label_Descripcion.Visible = value;
            this.label_URL.Visible = value;
            this.textBox_Descripcion.Visible = value;
            this.textBox_URL.Visible = value;
        }
        #endregion

        #region Región: Eventos Comunes
        /// <summary>
        /// Evento que surge cuando se hace clic en el botón Eliminar
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_Eliminar_Click(object sender, EventArgs e)
        {
            FuenteRSS pFuente = this.FuenteSeleccionada();
            if (!this.iListaFuenteRSSAgregar.Contains(pFuente))
            {
                DialogResult result = MessageBox.Show("¿Está seguro que desea eliminar la fuente junto con sus Banner Asociados?", "Atención",
                                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    this.iListaFuenteRSSEliminar.Add(pFuente);
                    this.iListaFuenteRSS.Remove(pFuente);
                }
            }
            else
            {
                this.iListaFuenteRSSAgregar.Remove(pFuente);
                this.iListaFuenteRSS.Remove(pFuente);
            }
            this.ActualizarDGV();
            this.ActivarAceptar();
        }

        /// <summary>
        /// Evento que surge cuando se hace clic en el botón Aceptar
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_Aceptar_Click(object sender, EventArgs e)
        {
            AdministracionRSS admininistracionRSS = new AdministracionRSS();
            admininistracionRSS.ListaRSSActualizar = this.iListaFuenteRSSActualizar;
            admininistracionRSS.ListaRSSEliminar = this.iListaFuenteRSSEliminar;
            admininistracionRSS.ListaRSSAgregar = this.iListaFuenteRSSAgregar;
            if (this.iNecesitaSeleccionar)
            {
                FuenteRSS pFuente = this.FuenteSeleccionada();
                if (this.iListaFuenteRSSAgregar.Contains(pFuente))
                {
                    this.iListaFuenteRSSAgregar.Remove(pFuente);
                    this.backgroundWorker_FuenteRSSSeleccion.RunWorkerAsync(pFuente);
                }
                else
                {
                    ((Form_Configuracion_Banner)this.Owner).ActualizarFuente(pFuente);
                }
            }
            this.backgroundWorker_Fuentes.RunWorkerAsync(admininistracionRSS);
            this.Hide();
        }

        /// <summary>
        /// Evento que surge cuando se hace clic en el botón Volver
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_Volver_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Está seguro que desea perder todos los cambios?\nSe perderán modificaciones, eliminaciones y nuevas Fuentes", "Atención",
                                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Evento que surge cuando el mouse se posicion arriba del botón Eliminar
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_Eliminar_MouseHover(object sender, EventArgs e)
        {
            this.label_Información.Visible = true;
        }

        /// <summary>
        /// Evento que surge cuando el mouse sale de arriba del botón Eliminar
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_Eliminar_MouseLeave(object sender, EventArgs e)
        {
            this.label_Información.Visible = false;
        }

        /// <summary>
        /// Evento que surge cuando cambia la selección de la fila
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            bool auxiliar = this.dataGridView.RowCount > 0;
            this.button_Modificar.Enabled = auxiliar;
            this.button_Eliminar.Enabled = auxiliar;
            this.textBox_Descripcion.Enabled = auxiliar;
            this.textBox_URL.Enabled = auxiliar;
        }

        /// <summary>
        /// Evento que surge cuando se sale del textBox_URL
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void textBox_URL_Leave(object sender, EventArgs e)
        {
            this.button_AceptarNuevo.Enabled = (this.textBox_URL.Text != "") && (this.textBox_Descripcion.Text != "");
            this.button_Modificar.Enabled = (this.textBox_URL.Text != "") && (this.textBox_Descripcion.Text != "");
            if (textBox_URL.Text != "")
            {
                Logging.Logger.Info("Inicio de obtencion RSS en Segundo plano de URI: "+textBox_URL.Text);
                this.backgroundWorker_ValorRSS.RunWorkerAsync(this.textBox_URL.Text);
            }            
            this.CampoCompleto(this.pictureBox_ComprobacionURL, this.textBox_URL.Text != "");
        }

        /// <summary>
        /// Evento que surge cuando se sale del textBox_Descripción
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void textBox_Descripcion_Leave(object sender, EventArgs e)
        {
            this.button_AceptarNuevo.Enabled = (this.textBox_URL.Text != "") && (this.textBox_Descripcion.Text != "");
            this.button_Modificar.Enabled = (this.textBox_URL.Text != "") && (this.textBox_Descripcion.Text != "");
            this.CampoCompleto(this.pictureBox_ComprobacionDescripcion, this.textBox_Descripcion.Text != "");
        }

        /// <summary>
        /// Evento que surge al hacer clic en el botón Nuevo
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_Nuevo_Click(object sender, EventArgs e)
        {
            this.HabilitarBotones(false);
            this.button_AceptarNuevo.Visible = true;
            this.button_AceptarNuevo.Enabled = true;
            this.textBox_Descripcion.Text = "";
            this.textBox_URL.Text = "";
            this.VisualizarEdicionRSS(true);
        }

        /// <summary>
        /// Evento que surge cuando se hace clic en el botón Agregar
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_AceptarAgregar_Click(object sender, EventArgs e)
        {
            FuenteRSS nuevaFuente = new FuenteRSS()
            {
                Codigo = 0,
                Valor = this.iValorRSS,
                Descripcion = this.textBox_Descripcion.Text,
                URL = this.textBox_URL.Text
            };
            this.iListaFuenteRSSAgregar.Add(nuevaFuente);
            this.iListaFuenteRSS.Add(nuevaFuente);
            this.HabilitarBotones(true);
            this.ActualizarDGV();
            this.VisualizarEdicionRSS(false);
            this.ActivarAceptar();
        }

        /// <summary>
        /// Evento que surge cuando se hace clic en el botón Modificar
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_Modificar_Click(object sender, EventArgs e)
        {
            FuenteRSS pFuente = this.FuenteSeleccionada();
            this.textBox_URL.Text = pFuente.URL;
            this.textBox_Descripcion.Text = pFuente.Descripcion;
            this.iValorRSS = pFuente.Valor;
            this.VisualizarEdicionRSS(true);
            this.HabilitarBotones(false);
            this.button_AceptarModificar.Visible = true;
            this.button_AceptarModificar.Enabled = true;
        }

        /// <summary>
        /// Evento que surge al hacer clic en el Aceptar luego del modificar
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_AceptarModificar_Click(object sender, EventArgs e)
        {
            FuenteRSS pFuente = this.FuenteSeleccionada();
            pFuente.URL = this.textBox_URL.Text;
            pFuente.Descripcion = this.textBox_Descripcion.Text;
            pFuente.Valor = this.iValorRSS;
            this.VisualizarEdicionRSS(true);
            if (!this.iListaFuenteRSSAgregar.Contains(pFuente))
            {
                this.iListaFuenteRSSActualizar.Add(pFuente);
            }
            this.HabilitarBotones(true);
            this.ActualizarDGV();
            this.VisualizarEdicionRSS(false);
            this.ActivarAceptar();
        }

        /// <summary>
        /// Evento que surge al hacer clic en el Cancelar
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void button_Cancelar_Click(object sender, EventArgs e)
        {
            this.textBox_Descripcion.Text = "";
            this.textBox_URL.Text = "";
            this.HabilitarBotones(true);
            this.VisualizarEdicionRSS(false);
        }
        #endregion

        #region Región: Procesos Segundo Plano
        /// <summary>
        /// Evento que surge cuando el Proceso en segundo plano empieza trabajar para guardar/modificar y actualizar Fuentes
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void backgroundWorker_Fuentes_DoWork(object sender, DoWorkEventArgs e)
        {
            AdministracionRSS administracionRSS = (AdministracionRSS)e.Argument;
            foreach(FuenteRSS pFuente in administracionRSS.ListaRSSActualizar)
            {
                ControladorFuente.Modificar(pFuente);
            }
            foreach (FuenteRSS pFuente in administracionRSS.ListaRSSEliminar)
            {
                ControladorFuente.Eliminar(pFuente);
            }
            foreach (FuenteRSS pFuente in administracionRSS.ListaRSSAgregar)
            {
                ControladorFuente.Agregar(pFuente);
            }
        }

        /// <summary>
        /// Evento que surge cuando el Proceso en segundo plano termina de trabajar para actualizar y guardar Fuentes
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void backgroundWorker_Fuentes_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(this.Owner.GetType() == typeof(Form_Configuracion_Banner))
            {
                ((Form_Configuracion_Banner)this.Owner).ActivarComboBox();
            }
        }

        /// <summary>
        /// Evento que surge cuando el Proceso en segundo plano obtiene las fuentes RSS
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void backgroundWorker_ObtenerRSS_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ControladorFuente.ObtenerFuentes(new FuenteRSS());
        }

        /// <summary>
        /// Evento que surge cuando el Proceso en segundo plano termina de trabajar de obtener las fuentes RSS
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void backgroundWorker_ObtenerRSS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error == null)
            {
                this.iListaFuenteRSS = ((List<IFuente>)e.Result).ConvertAll<FuenteRSS>(obj => (FuenteRSS)obj);
                this.dataGridView.DataSource = typeof(List<FuenteRSS>);
                this.dataGridView.DataSource = this.iListaFuenteRSS;
                this.ActualizarDGV();
            }
        }

        /// <summary>
        /// Evento que surge cuando el Proceso en segundo plano empieza trabajar para guardar el RSS y devolverlo
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void backgroundWorker_FuenteRSSSeleccion_DoWork(object sender, DoWorkEventArgs e)
        {
            FuenteRSS pFuente = (FuenteRSS)e.Argument;
            ControladorFuente.Agregar(pFuente);
            e.Result = pFuente;
        }

        /// <summary>
        /// Evento que surge cuando el Proceso en segundo plano termina de trabajar para actualizar y guardar Fuentes
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void backgroundWorker_FuenteRSSSeleccion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ((Form_Configuracion_Banner)this.Owner).ActualizarFuente((FuenteRSS)e.Result);
        }

        /// <summary>
        /// Evento que surge cuando el Proceso en segundo plano comienza a obtener el valor de la fuente de RSS
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void backgroundWorker_ValorRSS_DoWork(object sender, DoWorkEventArgs e)
        {
            FuenteRSS pFuente = new FuenteRSS() { URL = (string)e.Argument };
            try
            {
                pFuente.Valor = pFuente.ValorActualizado();
                e.Result = pFuente.Valor;
            }
            catch(Exception ex)
            {
                e.Result = ex;
            }
        }

        /// <summary>
        /// Evento que surge cuando el Proceso en segundo plano termina de obtener el valor del RSS
        /// </summary>
        /// <param name="sender">Objeto que  envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void backgroundWorker_ValorRSS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Exception errorProceso = e.Error;
            if(e.Result.GetType().IsSubclassOf(typeof(Exception)))
            {
                errorProceso = (Exception) e.Result;
            }
            if (errorProceso != null)
            {
                this.textBox_URL.Text = "";
                this.iValorRSS = "";
                MessageBox.Show("Se encontró el siguiente problema al obtener el valor de la Fuente RSS:\n" + errorProceso.Message);
                this.CampoCompleto(this.pictureBox_ComprobacionURL, false);
            }
            else
            {
                this.button_Modificar.Enabled = (this.textBox_URL.Text != "") && (this.textBox_Descripcion.Text != "");
                this.button_AceptarNuevo.Enabled = (this.textBox_URL.Text != "") && (this.textBox_Descripcion.Text != "");
                this.iValorRSS = ((string)e.Result);
                if (this.iValorRSS == null) { this.iValorRSS = ""; }
            }
        }
        #endregion

        /// <summary>
        /// Clase responsable de la adminsitración de Fuentes RSS (Listas de eliminar, actualizar y agregar)
        /// </summary>
        private class AdministracionRSS
        {
            private List<FuenteRSS> iListaRSSActualizar;
            private List<FuenteRSS> iListaRSSEliminar;
            private List<FuenteRSS> iListaRSSAgregar;

            public AdministracionRSS()
            {
                this.iListaRSSActualizar = new List<FuenteRSS>();
                this.iListaRSSEliminar = new List<FuenteRSS>();
                this.iListaRSSAgregar = new List<FuenteRSS>();
            }

            /// <summary>
            /// Get/Set de la Lista de RSS a Actualizar
            /// </summary>
            public List<FuenteRSS> ListaRSSActualizar
            {
                get { return this.iListaRSSActualizar; }
                set { this.iListaRSSActualizar = value; }
            }

            /// <summary>
            /// Get/Set de la Lista de RSS a Agregar
            /// </summary>
            public List<FuenteRSS> ListaRSSAgregar
            {
                get { return this.iListaRSSAgregar; }
                set { this.iListaRSSAgregar = value; }
            }

            /// <summary>
            /// Get/Set de la Lista de RSS a Eliminar
            /// </summary>
            public List<FuenteRSS> ListaRSSEliminar
            {
                get { return this.iListaRSSEliminar; }
                set { this.iListaRSSEliminar = value; }

            }
        }
    }
}
