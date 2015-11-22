/*
    Copyright (c) 2015-2016, Raven( craven@crowz.kr )
    All rights reserved.
    
	This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
 */

using System;
using System.IO;
using System.IO.Ports;

namespace ssterm
{
	public partial class WindowSetting : Gtk.Window
	{
		SerialPort _serial;
		public delegate  void DisplayStatusHandler();
		public event DisplayStatusHandler dis;

		public WindowSetting (SerialPort serial) :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			_serial = serial;
			entry2.Text = serial.PortName;
			entry3.Text = "" + serial.BaudRate;
			entry5.Text = "" + serial.StopBits;
			entry6.Text = "" + serial.DataBits;
		}


		protected void OnSaveActionActivated (object sender, EventArgs e)
		{
			//throw new NotImplementedException ();
			_serial.PortName = entry2.Text;
			_serial.BaudRate = Convert.ToInt32 (entry3.Text);
			_serial.DataBits = Convert.ToInt32 (entry6.Text);
			if (dis != null)
				dis ();
			this.Destroy ();
		}
		protected void OnDeleteActionActivated (object sender, EventArgs e)
		{
			this.Destroy ();
		}
	}
}

