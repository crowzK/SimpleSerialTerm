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
using ssterm;
using System.Text;
using System.Threading;
using System.Reflection;

public partial class MainWindow: Gtk.Window
{
	SerialPort _serialPort;// = new SerialPort();
	Boolean portStatus = false;
	public Thread workerThread;



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

		DisplayStatus ();
		disconnectAction.Visible = false;
		workerThread = new Thread(DoReceive);
		workerThread.Start ();

	}
	~MainWindow()
	{
		_serialPort.Close ();
		workerThread.Abort ();
	}

	public virtual void Dispose()
	{
		_serialPort.Close ();
		workerThread.Abort ();
		// Suppress finalization.
		GC.SuppressFinalize(this);
	}

	public void DoReceive()
	{
		while (true) {
			byte[] recvByte = new byte[1000];
			try {
					_serialPort.Read (recvByte, 0, 1000);
					Gtk.Application.Invoke(delegate
					{
						consoleTextView.Buffer.Text += Encoding.ASCII.GetString (recvByte);
					});
				Thread.Sleep(1);
			} catch {

			}
		}
	}

	private void DisplayStatus()
	{
		if (portStatus == true)
			status.Text = "OPEN";
		else
			status.Text = "CLOSE";
		COM.Text = _serialPort.PortName;
		Baudrate.Text = "" + _serialPort.BaudRate;

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
		winset.dis += DisplayStatus;
		winset.Show ();
	}

	protected void OnConnectActionActivated (object sender, EventArgs e)
	{
	//	throw new NotImplementedException ();
		try
		{
		_serialPort.Open ();
		}
		catch(Exception ex) 
		{
			consoleTextView.Buffer.Text += ex.Message+"\n";
			return;
		}
		connectAction.Visible = false;
		disconnectAction.Visible = true;
		portStatus = true;
		DisplayStatus ();
	}
	protected void OnDisconnectActionActivated (object sender, EventArgs e)
	{
		//throw new NotImplementedException ();
		try
		{
			_serialPort.Close ();
		}catch(Exception ex) {
			consoleTextView.Buffer.Text += ex.Message + "\n";
		}
		finally {
			connectAction.Visible = true;
			disconnectAction.Visible = false;
			portStatus = false;
			DisplayStatus ();
		}
	}

	protected void OnConsoleTextViewKeyReleaseEvent (object o, KeyReleaseEventArgs args)
	{
		//throw new NotImplementedException ();
		Int16 key = (Int16)args.Event.KeyValue;
		byte[] keyAscii = new byte[1];

		if (key < 0) {
			//consoleTextView.Buffer.Text += args.Event.Key;

			if (args.Event.Key == Gdk.Key.Delete) {
				keyAscii [0] = 127;
			} else if (args.Event.Key == Gdk.Key.Return) {
				keyAscii = new byte[2];
				keyAscii [0] = 10;
				keyAscii [1] = 13;
			} else if (args.Event.Key == Gdk.Key.Escape) {
				keyAscii [0] = 27;
			} else if (args.Event.Key == Gdk.Key.BackSpace) {
				keyAscii [0] = 08;
			} else {
				return;
			}

		} else {
			keyAscii[0] = (byte)key;
		}

		if (portStatus == true) {
			_serialPort.Write (keyAscii, 0, keyAscii.Length);
		}

	}
}
