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
using Gtk;
using System.IO;
using System.Windows;
using System.IO.Ports;
using testapp;

public partial class MainWindow: Gtk.Window
{
	SerialPort _serialPort;// = new SerialPort();
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		_serialPort = new SerialPort();
		_serialPort.PortName = "/dev/ttyUSB0";
		_serialPort.BaudRate = 115200;
		_serialPort.DataBits = 8;
		_serialPort.Parity = Parity.None;
		_serialPort.StopBits = StopBits.One;
		_serialPort.ReadTimeout = 500;
		_serialPort.WriteTimeout = 500;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnOpenAction1Activated (object sender, EventArgs e)
	{
		consoleTextView.Buffer.Text = "";
		//throw new NotImplementedException ();
		Gtk.FileChooserDialog filechooser =
			new Gtk.FileChooserDialog("Choose the file to open",
				this,
				FileChooserAction.Open,
				"Cancel",ResponseType.Cancel,
				"Open",ResponseType.Accept);

		if (filechooser.Run() == (int)ResponseType.Accept) 
		{
			System.IO.FileStream file = System.IO.File.OpenRead(filechooser.Filename);
			file.Close();
		}
		consoleTextView.Buffer.Text = filechooser.Filename;

		filechooser.Destroy();
	}


	protected void OnPreferencesActionActivated (object sender, EventArgs e)
	{
		//throw new NotImplementedException ();
		WindowSetting winset = new WindowSetting (_serialPort);
		winset.Show ();
	}

}
