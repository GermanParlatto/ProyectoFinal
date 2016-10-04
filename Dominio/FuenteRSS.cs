﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("UI")]

namespace Dominio
{
    class FuenteRSS : IFuente
    {
        private string iURL;
        private string iDescripcion;
        private string iValorAnterior;
        private int iCodigo;

        /// <summary>
        /// Constructor del RSS
        /// </summary>
        /// <param name="pURL">Texto propio</param>
        /// <param name="pDescripcion">Descripción del RSS</param>
        public FuenteRSS()
        {
            this.iValorAnterior = "";
        }

        /// <summary>
        /// Get/Set del código de la Fuente RSS
        /// </summary>
        public int Codigo
        {
            get { return this.iCodigo; }
            set { this.iCodigo = value; }
        } 

        /// <summary>
        /// Get/Set de la URL del RSS
        /// </summary>
        public string URL
        {
            get { return this.iURL; }
            set { this.iURL = value; }
        }

        /// <summary>
        /// Get/Set de la descripcion de la Fuente
        /// </summary>
        public string Descripcion
        {
            get { return this.iDescripcion; }
            set { this.iDescripcion = value; }
        }

        /// <summary>
        /// Get/Set del valor
        /// </summary>
        public string Valor
        {
            get { return this.iValorAnterior; }
            set { this.iValorAnterior = value; }
        }

        /// <summary>
        /// Get Texto anterior del RSS   
        /// </summary>
        /// <returns>Tipo de dato string que representa el texto anterior de la fuente RSS</returns>
        public string Texto()
        {
            try {
                string aux = this.ValorActualizado();
                if (!(aux == ""))
                {
                    this.iValorAnterior = aux;
                }
                return this.iValorAnterior;
            }
            catch (Exception ex)
            {
                Logging.Logger.Info("Problemas de conectividad, excepcion capturada:",ex);
                return this.iValorAnterior;
            }
        }

        /// <summary>
        /// Actualiza la fuente RSS
        /// </summary>
        public string ValorActualizado()
        {
            IRssReader mRssReader = new RawXmlRssReader();
            IEnumerable<RssItem> mItmes = mRssReader.Read(this.URL);
            StringBuilder resultado = new StringBuilder("");
            foreach (RssItem pItem in mItmes)
            {
                resultado.Append(pItem.Title + " // ");
            }
            return resultado.ToString();
        }

        /// <summary>
        /// Determina si la fuenteRSS debe actualizarse
        /// </summary>
        /// <returns>Tipo de dato bool que representa si debe actualizarse o no la fuenteRSS</returns>
        public bool Actualizable()
        {
            return true;
        }

        /// <summary>
        /// Determina si dos fuentes son iguales
        /// </summary>
        /// <param name="other">Otra Fuente a comparar</param>
        /// <returns>Tipo de dato bool que representa si dos Fuentes son iguales</returns>
        public bool Equals(IFuente other)
        {
            return (other.GetType() == this.GetType()) && (this.Codigo == other.Codigo);
        }

        /// <summary>
        /// Actualiza la fuente RSS con su último valor
        /// </summary>
        public void Actualizar()
        {
            this.Texto();
        }
    }
}
