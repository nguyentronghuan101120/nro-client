namespace Game1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using UnityEngine;
    
    public class Session_ME2 : ISession
    {
    	public class Sender
    	{
    		public List<Message> sendingMessage;
    
    		public Sender()
    		{
    			sendingMessage = new List<Message>();
    		}
    
    		public void AddMessage(Message message)
    		{
    			sendingMessage.Add(message);
    		}
    
    		public void run()
    		{
    			while (connected)
    			{
    				try
    				{
    					if (getKeyComplete)
    					{
    						while (sendingMessage.Count > 0)
    						{
    							Message m = sendingMessage[0];
    							doSendMessage(m);
    							sendingMessage.RemoveAt(0);
    						}
    					}
    					try
    					{
    						Thread.Sleep(5);
    					}
    					catch (Exception ex)
    					{
    						Cout.LogError(ex.ToString());
    					}
    				}
    				catch (Exception)
    				{
    					Res.outz("error send message! ");
    				}
    			}
    		}
    	}
    
    	private class MessageCollector
    	{
    		public void run()
    		{
    			try
    			{
    				while (connected)
    				{
    					Message message = readMessage();
    					if (message == null)
    					{
    						break;
    					}
    					try
    					{
    						if (message.command == -27)
    						{
    							getKey(message);
    						}
    						else
    						{
    							onRecieveMsg(message);
    						}
    					}
    					catch (Exception)
    					{
    						Cout.println("LOI NHAN  MESS THU 1");
    					}
    					try
    					{
    						Thread.Sleep(5);
    					}
    					catch (Exception)
    					{
    						Cout.println("LOI NHAN  MESS THU 2");
    					}
    				}
    			}
    			catch (Exception ex3)
    			{
    				Debug.Log("error read message!");
    				Debug.Log(ex3.Message.ToString());
    			}
    			if (!connected)
    			{
    				return;
    			}
    			if (messageHandler != null)
    			{
    				if (currentTimeMillis() - timeConnected > 500)
    				{
    					messageHandler.onDisconnected(isMainSession);
    				}
    				else
    				{
    					messageHandler.onConnectionFail(isMainSession);
    				}
    			}
    			if (sc != null)
    			{
    				cleanNetwork();
    			}
    		}
    
    		private void getKey(Message message)
    		{
    			try
    			{
    				sbyte b = message.reader().readSByte();
    				key = new sbyte[b];
    				for (int i = 0; i < b; i++)
    				{
    					key[i] = message.reader().readSByte();
    				}
    				for (int j = 0; j < key.Length - 1; j++)
    				{
    					ref sbyte reference = ref key[j + 1];
    					reference ^= key[j];
    				}
    				getKeyComplete = true;
    				GameMidlet.IP2 = message.reader().readUTF();
    				GameMidlet.PORT2 = message.reader().readInt();
    				GameMidlet.isConnect2 = ((message.reader().readByte() != 0) ? true : false);
    				if (isMainSession && GameMidlet.isConnect2)
    				{
    					GameCanvas.connect2();
    				}
    			}
    			catch (Exception)
    			{
    			}
    		}
    
		private Message readMessage2(sbyte cmd)
		{
			try
			{
				// Check if dis is null or connection is closed
				if (dis == null || !connected)
				{
					return null;
				}
				
				int num = readKey(dis.ReadSByte()) + 128;
				if (dis == null || !connected) return null;
				
				int num2 = readKey(dis.ReadSByte()) + 128;
				if (dis == null || !connected) return null;
				
				int num3 = readKey(dis.ReadSByte()) + 128;
				if (dis == null || !connected) return null;
				
				int num4 = (num3 * 256 + num2) * 256 + num;
				Cout.LogError("SIZE = " + num4);
				
				// Validate size before allocating array (max 10MB)
				if (num4 < 0 || num4 > 10485760)
				{
					Debug.Log("Invalid message size: " + num4);
					return null;
				}
				
				sbyte[] array = new sbyte[num4];
				if (dis == null || !connected) return null;
				
				byte[] src = dis.ReadBytes(num4);
				
				// Check if we received the expected number of bytes
				if (src == null || src.Length != num4)
				{
					Debug.Log("Failed to read expected bytes. Expected: " + num4 + ", Got: " + (src != null ? src.Length : 0));
					return null;
				}
				
				array = ArrayCast.cast(src);
				recvByteCount += 5 + num4;
				int num6 = recvByteCount + sendByteCount;
				strRecvByteCount = num6 / 1024 + "." + num6 % 1024 / 102 + "Kb";
				if (getKeyComplete)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = readKey(array[i]);
					}
				}
				return new Message(cmd, array);
			}
			catch (ObjectDisposedException)
			{
				// Socket was closed - this is expected during disconnect, don't log
				return null;
			}
			catch (IOException)
			{
				// IO errors during disconnect are expected
				if (connected)
				{
					Debug.Log("Error reading message2: IO Error");
				}
				return null;
			}
			catch (ThreadAbortException)
			{
				// Thread is being aborted - don't log, just exit
				return null;
			}
			catch (Exception ex)
			{
				// Only log unexpected errors when we're supposed to be connected
				if (connected)
				{
					Debug.Log("Error reading message2: " + ex.Message);
				}
				return null;
			}
		}
    
		private Message readMessage()
		{
			try
			{
				// Check if dis is null or connection is closed
				if (dis == null || !connected)
				{
					return null;
				}
				
				sbyte b = dis.ReadSByte();
				if (getKeyComplete)
				{
					b = readKey(b);
				}
				if (b == -32 || b == -66 || b == 11 || b == -67 || b == -74 || b == -87)
				{
					return readMessage2(b);
				}
				int num;
				if (getKeyComplete)
				{
					sbyte b2 = dis.ReadSByte();
					sbyte b3 = dis.ReadSByte();
					num = ((readKey(b2) & 0xFF) << 8) | (readKey(b3) & 0xFF);
				}
				else
				{
					sbyte b4 = dis.ReadSByte();
					sbyte b5 = dis.ReadSByte();
					num = (b4 & 0xFF00) | (b5 & 0xFF);
				}
				
				// Validate size before allocating array (max 10MB)
				if (num < 0 || num > 10485760)
				{
					Debug.Log("Invalid message size: " + num);
					return null;
				}
				
				sbyte[] array = new sbyte[num];
				byte[] src = dis.ReadBytes(num);
				
				// Check if we received the expected number of bytes
				if (src == null || src.Length != num)
				{
					Debug.Log("Failed to read expected bytes. Expected: " + num + ", Got: " + (src != null ? src.Length : 0));
					return null;
				}
				
                array = ArrayCast.cast(src);
                recvByteCount += 5 + num;
				int num4 = recvByteCount + sendByteCount;
				strRecvByteCount = num4 / 1024 + "." + num4 % 1024 / 102 + "Kb";
				if (getKeyComplete)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = readKey(array[i]);
					}
				}
				return new Message(b, array);
			}
			catch (ObjectDisposedException)
			{
				// Socket was closed - this is expected during disconnect, don't log
				return null;
			}
			catch (IOException ioEx)
			{
				// IO errors during disconnect are expected
				if (!connected)
				{
					return null;
				}
				Debug.Log("IO Error reading message: " + ioEx.Message);
			}
			catch (ThreadAbortException)
			{
				// Thread is being aborted - don't log, just exit
				return null;
			}
			catch (Exception ex)
			{
				// Only log unexpected errors when we're supposed to be connected
				if (connected)
				{
					Debug.Log("Error reading message: " + ex.Message);
				}
			}
			return null;
		}
    	}
    
    	protected static Session_ME2 instance = new Session_ME2();
    
    	private static NetworkStream dataStream;
    
    	private static BinaryReader dis;
    
    	private static BinaryWriter dos;
    
    	public static IMessageHandler messageHandler;
    
    	public static bool isMainSession = true;
    
    	private static TcpClient sc;
    
    	public static bool connected;
    
    	public static bool connecting;
    
    	private static Sender sender = new Sender();
    
    	public static Thread initThread;
    
    	public static Thread collectorThread;
    
    	public static Thread sendThread;
    
    	public static int sendByteCount;
    
    	public static int recvByteCount;
    
    	private static bool getKeyComplete;
    
    	public static sbyte[] key = null;
    
    	private static sbyte curR;
    
    	private static sbyte curW;
    
    	private static int timeConnected;
    
    	private long lastTimeConn;
    
    	public static string strRecvByteCount = string.Empty;
    
    	public static bool isCancel;
    
    	private string host;
    
    	private int port;
    
    	private long timeWaitConnect;
    
    	public static MyVector recieveMsg = new MyVector();
    
    	public Session_ME2()
    	{
    		Debug.Log("init Session_ME");
    	}
    
    	public void clearSendingMessage()
    	{
    		sender.sendingMessage.Clear();
    	}
    
    	public static Session_ME2 gI()
    	{
    		if (instance == null)
    		{
    			instance = new Session_ME2();
    		}
    		return instance;
    	}
    
    	public bool isConnected()
    	{
    		return connected && sc != null && dis != null;
    	}
    
    	public void setHandler(IMessageHandler msgHandler)
    	{
    		messageHandler = msgHandler;
    	}
    
    	public void connect(string host, int port)
    	{
    		if (!connected && !connecting && mSystem.currentTimeMillis() >= timeWaitConnect)
    		{
    			timeWaitConnect = mSystem.currentTimeMillis() + 50;
    			this.host = host;
    			this.port = port;
    			getKeyComplete = false;
    			close();
    			Debug.Log("connecting...!");
    			Debug.Log("host: " + host);
    			Debug.Log("port: " + port);
    			initThread = new Thread(NetworkInit);
    			initThread.Start();
    		}
    	}
    
    	private void NetworkInit()
    	{
    		isCancel = false;
    		connecting = true;
    		Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
    		connected = true;
    		try
    		{
    			doConnect(host, port);
    			messageHandler.onConnectOK(isMainSession);
    		}
    		catch (Exception)
    		{
    			if (messageHandler != null)
    			{
    				close();
    				messageHandler.onConnectionFail(isMainSession);
    			}
    		}
    	}
    
    	public void doConnect(string host, int port)
    	{
    		sc = new TcpClient();
    		sc.Connect(host, port);
    		dataStream = sc.GetStream();
    		dis = new BinaryReader(dataStream, new UTF8Encoding());
    		dos = new BinaryWriter(dataStream, new UTF8Encoding());
    		sendThread = new Thread(sender.run);
    		sendThread.Start();
    		MessageCollector @object = new MessageCollector();
    		Cout.LogError("new -----");
    		collectorThread = new Thread(@object.run);
    		collectorThread.Start();
    		timeConnected = currentTimeMillis();
    		connecting = false;
    		doSendMessage(new Message(-27));
    	}
    
    	public void sendMessage(Message message)
    	{
    		Res.outz("SEND MSG: " + message.command);
    		sender.AddMessage(message);
    	}
    
    	private static void doSendMessage(Message m)
    	{
    		sbyte[] data = m.getData();
    		try
    		{
    			if (getKeyComplete)
    			{
    				sbyte value = writeKey(m.command);
    				dos.Write(value);
    			}
    			else
    			{
    				dos.Write(m.command);
    			}
    			if (data != null)
    			{
    				int num = data.Length;
    				if (getKeyComplete)
    				{
    					int num2 = writeKey((sbyte)(num >> 8));
    					dos.Write((sbyte)num2);
    					int num3 = writeKey((sbyte)(num & 0xFF));
    					dos.Write((sbyte)num3);
    				}
    				else
    				{
    					dos.Write((ushort)num);
    				}
    				if (getKeyComplete)
    				{
    					for (int i = 0; i < data.Length; i++)
    					{
    						sbyte value2 = writeKey(data[i]);
    						dos.Write(value2);
    					}
    				}
    				sendByteCount += 5 + data.Length;
    			}
    			else
    			{
    				if (getKeyComplete)
    				{
    					int num4 = 0;
    					int num5 = writeKey((sbyte)(num4 >> 8));
    					dos.Write((sbyte)num5);
    					int num6 = writeKey((sbyte)(num4 & 0xFF));
    					dos.Write((sbyte)num6);
    				}
    				else
    				{
    					dos.Write((ushort)0);
    				}
    				sendByteCount += 5;
    			}
    			dos.Flush();
    		}
    		catch (Exception ex)
    		{
    			Debug.Log(ex.StackTrace);
    		}
    	}
    
	public static sbyte readKey(sbyte b)
	{
		// Check if key is null
		if (key == null || key.Length == 0)
		{
			return b;
		}
		
		sbyte[] array = key;
		sbyte num = curR;
		curR = (sbyte)(num + 1);
		sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
		if (curR >= key.Length)
		{
			curR %= (sbyte)key.Length;
		}
		return result;
	}
    
	public static sbyte writeKey(sbyte b)
	{
		// Check if key is null
		if (key == null || key.Length == 0)
		{
			return b;
		}
		
		sbyte[] array = key;
		sbyte num = curW;
		curW = (sbyte)(num + 1);
		sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
		if (curW >= key.Length)
		{
			curW %= (sbyte)key.Length;
		}
		return result;
	}
    
    	public static void onRecieveMsg(Message msg)
    	{
    		if (Thread.CurrentThread.Name == Main.mainThreadName)
    		{
    			messageHandler.onMessage(msg);
    		}
    		else
    		{
    			recieveMsg.addElement(msg);
    		}
    	}
    
    	public static void update()
    	{
    		while (recieveMsg.size() > 0)
    		{
    			Message message = (Message)recieveMsg.elementAt(0);
    			if (Controller.isStopReadMessage)
    			{
    				break;
    			}
    			if (message == null)
    			{
    				recieveMsg.removeElementAt(0);
    				break;
    			}
    			messageHandler.onMessage(message);
    			recieveMsg.removeElementAt(0);
    		}
    	}
    
    	public void close()
    	{
    		cleanNetwork();
    	}
    
	private static void cleanNetwork()
	{
		key = null;
		curR = 0;
		curW = 0;
		try
		{
			// Set connected to false first to signal threads to exit
			connected = false;
			connecting = false;
			
			// Close streams first to unblock any pending reads
			// This will cause threads to exit naturally
			try
			{
				if (dis != null)
				{
					dis.Close();
					dis = null;
				}
			}
			catch (Exception) { }
			
			try
			{
				if (dos != null)
				{
					dos.Close();
					dos = null;
				}
			}
			catch (Exception) { }
			
			try
			{
				if (dataStream != null)
				{
					dataStream.Close();
					dataStream = null;
				}
			}
			catch (Exception) { }
			
			try
			{
				if (sc != null)
				{
					sc.Close();
					sc = null;
				}
			}
			catch (Exception) { }
			
			// Give threads a moment to exit naturally after streams are closed
			// Wait for threads to finish naturally (with timeout)
			if (collectorThread != null && collectorThread.IsAlive)
			{
				try
				{
					if (!collectorThread.Join(100)) // Wait 100ms
					{
						collectorThread.Abort();
					}
				}
				catch (Exception) { }
			}
			collectorThread = null;
			
			if (sendThread != null && sendThread.IsAlive)
			{
				try
				{
					if (!sendThread.Join(100)) // Wait 100ms
					{
						sendThread.Abort();
					}
				}
				catch (Exception) { }
			}
			sendThread = null;
		}
		catch (Exception)
		{
		}
	}
    
    	public static int currentTimeMillis()
    	{
    		return Environment.TickCount;
    	}
    
    	public static byte convertSbyteToByte(sbyte var)
    	{
    		if (var > 0)
    		{
    			return (byte)var;
    		}
    		return (byte)(var + 256);
    	}
    
    	public static byte[] convertSbyteToByte(sbyte[] var)
    	{
    		byte[] array = new byte[var.Length];
    		for (int i = 0; i < var.Length; i++)
    		{
    			if (var[i] > 0)
    			{
    				array[i] = (byte)var[i];
    			}
    			else
    			{
    				array[i] = (byte)(var[i] + 256);
    			}
    		}
    		return array;
    	}
    }
}
