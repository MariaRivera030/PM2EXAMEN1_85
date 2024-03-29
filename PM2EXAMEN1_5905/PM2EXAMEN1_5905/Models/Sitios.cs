﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace PM2EXAMEN1_5905.Models
{
    public class Sitios
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public Byte[] foto { get; set; }
        public string fotoPath { get; set; }
        public Double latitud { get; set; }
        public Double longitud { get; set; }
        public string descripcion { get; set; }
    }
}
