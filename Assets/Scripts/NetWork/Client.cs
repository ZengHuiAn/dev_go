using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Snake;
using UnityEngine;
using XLua;

public struct ClientHeader
{
    public UInt32 Length;
    public UInt32 MessageID;
    public UInt32 SN;
}



namespace Snake
{
    [LuaCallCSharp]
    public class Client
    {

        
        private TcpClient client = null;

        private static Client getClient;

        public static Client GetClient
        {
            get
            {
                if (getClient == null)
                {
                    getClient = new Client();
                }
                return getClient;
            }
        }

        [LuaCallCSharp]
        public bool Connect(string host, int port)
        {
            //
            client = new TcpClient();

            try
            {
                client.NoDelay = true;
                client.Connect(host, port);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + "\n");
                return false;
            }

            return client.Connected;
        }
        [LuaCallCSharp]
        public LuaTable Read()
        {
            if (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                //
                System.IO.BinaryReader reader = new BinaryReader(stream);
                byte[] headerBuffer = new byte[12];
                reader.Read(headerBuffer, 0, headerBuffer.Length);
                ClientHeader header = ConvertHeader(headerBuffer);
                UInt32 length = 12;
                UInt32 bodyLength = header.Length - length;

                byte[] bodyBytes = new byte[bodyLength];
                using (MemoryStream m_stream = new MemoryStream())
                {
                    m_stream.Write(bodyBytes, 0, (int)m_stream.Length);
                    LuaTable msgTable = LuaController.Instance.LuaEnv.NewTable();
                    object body = amf.Decode(m_stream);
                    return LuaConvert.ConvertLuaTable(body);
                }
            }
            return null;
        }
        [LuaCallCSharp]
        public bool Send(uint cmd, uint sn, params object[] obs)
        {
            if (!this.client.Connected)
            {
                return false;
            }

            if (obs.Length == 0)
            {
                return false;
            }

            
            byte[] sendBuffer;
            if (obs.Length == 1)
            {
                if (obs[0].GetType() == typeof(LuaTable))
                {
                    Utils.utils.Log("LuaTable:------>>>", LuaConvert.ConvertObjects(obs[0] as LuaTable) );
                    object[] result = LuaConvert.ConvertObjects(obs[0] as LuaTable);
                    //amf.Encode()
                    Utils.utils.Log("result------->>>", result);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        amf.Encode(stream, result);
                        sendBuffer = stream.ToArray();
                        Utils.utils.Log(sendBuffer);
                    }
                    if (sendBuffer != null)
                    {
                        //header.Length = 12 + sendBuffer.Length;
                        var header = new ClientHeader();
                        header.Length = (uint)(12 + sendBuffer.Length);
                        header.Length = (uint)(12 + sendBuffer.Length);
                        header.SN = sn;
                        header.MessageID = cmd;
                        var bsHeader = ConvertNetHeader(header);
                        Utils.utils.Log("Header:", bsHeader);
                        Utils.utils.Log("body:", sendBuffer);
                        byte[] newBuffer = new byte[12 + sendBuffer.Length];
                        Array.Copy(bsHeader, 0, newBuffer, 0, bsHeader.Length);
                        Array.Copy(sendBuffer, 0, newBuffer, 12, sendBuffer.Length);
                        Utils.utils.Log("packBuffer:", newBuffer);
                        try
                        {
                            this.client.GetStream().Write(newBuffer, 0, newBuffer.Length);
                            return true;
                        }
                        catch (Exception e)
                        {
                            this.client.Close();
                            Debug.Log(e.Message);
                        }

                    }
                }
            }

            object[] encodes = new object[obs.Length];
            //amf.Encode()

            for (int i = 0; i < obs.Length; i++)
            {
                if (obs[i].GetType() == typeof(LuaTable))
                {
                    object[] result = LuaConvert.ConvertObjects(obs[0] as LuaTable);
                    encodes[i] = result;
                }
                else
                {
                    encodes[i] = obs[i];
                }
            }
            using (MemoryStream stream = new MemoryStream())
            {
                amf.Encode(stream, encodes);
                sendBuffer = stream.ToArray();
            }
            try
            {
                var header = new ClientHeader();
                header.Length = (uint)(12 + sendBuffer.Length);
                header.Length = (uint)(12 + sendBuffer.Length);
                header.SN = sn;
                header.MessageID = cmd;
                var bsHeader = ConvertNetHeader(header);
                Utils.utils.Log("Header:", bsHeader);
                Utils.utils.Log("body:", sendBuffer);
                byte[] newBuffer = new byte[bsHeader.Length + sendBuffer.Length];
                Array.Copy(bsHeader, 0, newBuffer, 0, bsHeader.Length);
                Array.Copy(sendBuffer, 0, newBuffer, 12, sendBuffer.Length);
                Utils.utils.Log("packBuffer:", newBuffer);
                this.client.GetStream().Write(newBuffer, 0, newBuffer.Length);
                return true;
            }
            catch (Exception e)
            {
                this.client.Close();
                Debug.Log(e.Message);
            }
            //amf.Encode()


            return false;
        }

        ClientHeader ConvertHeader(byte[] headerBuffer)
        {
            ClientHeader header = new ClientHeader();
            //
            byte[] lengthBytes = Client.CopyArray(headerBuffer, 0, 4);

            header.Length = BitConverter.ToUInt32(lengthBytes, 0);
            byte[] messageBytes = Client.CopyArray(headerBuffer, 4, 8); ;
            header.MessageID = BitConverter.ToUInt32(messageBytes, 0);
            byte[] snBytes = Client.CopyArray(headerBuffer, 8, 12);
            header.SN = BitConverter.ToUInt32(snBytes, 0);
            return header;
        }


        public static byte[] CopyArray(byte[] array, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            //
            byte[] temp = new byte[length];
            for (int i = startIndex, j = 0; i < endIndex && j < length; i++, j++)
            {
                temp[j] = array[i];
            }
            return temp;
        }
        /// <summary>
        /// 转换头部
        /// </summary>
        /// <returns></returns>
        byte[] ConvertNetHeader(ClientHeader header)
        {
            List<byte> buffer = new List<byte>();
            Type tp = header.GetType();

            System.Reflection.FieldInfo[] infos = tp.GetFields();
            object[] obs = new object[infos.Length];
            for (int i = 0; i < infos.Length; i += 1)
            {
                //
                obs[i] = infos[i].GetValue(header as object);
                var bs = BitConverter.GetBytes((uint)obs[i]);
                buffer.AddRange(bs);
            }
            return buffer.ToArray();
        }

        public void Close()
        {
            if (this.client !=null && this.client.Connected)
            {
                this.client.Close();
            }
        }
    }

}
