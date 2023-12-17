namespace Almacen
{
    partial class ProductoUbicaciones
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cboProducto = new System.Windows.Forms.ComboBox();
            this.cboUbicacion = new System.Windows.Forms.ComboBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.dtgProdUbicaion = new System.Windows.Forms.DataGridView();
            this.almacenDBDataSet4 = new Almacen.AlmacenDBDataSet4();
            this.productosBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.productosTableAdapter = new Almacen.AlmacenDBDataSet4TableAdapters.ProductosTableAdapter();
            this.almacenDBDataSet5 = new Almacen.AlmacenDBDataSet5();
            this.ubicacionesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ubicacionesTableAdapter = new Almacen.AlmacenDBDataSet5TableAdapters.UbicacionesTableAdapter();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dtgProdUbicaion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.almacenDBDataSet4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productosBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.almacenDBDataSet5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ubicacionesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cboProducto
            // 
            this.cboProducto.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.productosBindingSource, "ProductoID", true));
            this.cboProducto.DataSource = this.productosBindingSource;
            this.cboProducto.DisplayMember = "Nombre";
            this.cboProducto.FormattingEnabled = true;
            this.cboProducto.Location = new System.Drawing.Point(12, 95);
            this.cboProducto.Name = "cboProducto";
            this.cboProducto.Size = new System.Drawing.Size(381, 21);
            this.cboProducto.TabIndex = 0;
            this.cboProducto.ValueMember = "ProductoID";
            // 
            // cboUbicacion
            // 
            this.cboUbicacion.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.ubicacionesBindingSource, "UbicacionID", true));
            this.cboUbicacion.DataSource = this.ubicacionesBindingSource;
            this.cboUbicacion.DisplayMember = "Nombre";
            this.cboUbicacion.FormattingEnabled = true;
            this.cboUbicacion.Location = new System.Drawing.Point(418, 95);
            this.cboUbicacion.Name = "cboUbicacion";
            this.cboUbicacion.Size = new System.Drawing.Size(347, 21);
            this.cboUbicacion.TabIndex = 1;
            this.cboUbicacion.ValueMember = "UbicacionID";
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(12, 198);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(156, 39);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.Text = "AGREGAR";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // dtgProdUbicaion
            // 
            this.dtgProdUbicaion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgProdUbicaion.Location = new System.Drawing.Point(12, 268);
            this.dtgProdUbicaion.Name = "dtgProdUbicaion";
            this.dtgProdUbicaion.Size = new System.Drawing.Size(753, 225);
            this.dtgProdUbicaion.TabIndex = 3;
            // 
            // almacenDBDataSet4
            // 
            this.almacenDBDataSet4.DataSetName = "AlmacenDBDataSet4";
            this.almacenDBDataSet4.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // productosBindingSource
            // 
            this.productosBindingSource.DataMember = "Productos";
            this.productosBindingSource.DataSource = this.almacenDBDataSet4;
            // 
            // productosTableAdapter
            // 
            this.productosTableAdapter.ClearBeforeFill = true;
            // 
            // almacenDBDataSet5
            // 
            this.almacenDBDataSet5.DataSetName = "AlmacenDBDataSet5";
            this.almacenDBDataSet5.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // ubicacionesBindingSource
            // 
            this.ubicacionesBindingSource.DataMember = "Ubicaciones";
            this.ubicacionesBindingSource.DataSource = this.almacenDBDataSet5;
            // 
            // ubicacionesTableAdapter
            // 
            this.ubicacionesTableAdapter.ClearBeforeFill = true;
            // 
            // txtCantidad
            // 
            this.txtCantidad.Location = new System.Drawing.Point(12, 151);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(165, 20);
            this.txtCantidad.TabIndex = 4;
            // 
            // ProductoUbicaciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 534);
            this.Controls.Add(this.txtCantidad);
            this.Controls.Add(this.dtgProdUbicaion);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.cboUbicacion);
            this.Controls.Add(this.cboProducto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ProductoUbicaciones";
            this.Text = "ProductoUbicaciones";
            this.Load += new System.EventHandler(this.ProductoUbicaciones_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtgProdUbicaion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.almacenDBDataSet4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productosBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.almacenDBDataSet5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ubicacionesBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboProducto;
        private System.Windows.Forms.ComboBox cboUbicacion;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.DataGridView dtgProdUbicaion;
        private AlmacenDBDataSet4 almacenDBDataSet4;
        private System.Windows.Forms.BindingSource productosBindingSource;
        private AlmacenDBDataSet4TableAdapters.ProductosTableAdapter productosTableAdapter;
        private AlmacenDBDataSet5 almacenDBDataSet5;
        private System.Windows.Forms.BindingSource ubicacionesBindingSource;
        private AlmacenDBDataSet5TableAdapters.UbicacionesTableAdapter ubicacionesTableAdapter;
        private System.Windows.Forms.TextBox txtCantidad;
    }
}