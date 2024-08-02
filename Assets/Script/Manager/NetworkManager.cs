using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using static COM_PROTOCOL;

static public class COM_PROTOCOL
{
    public const byte COM_PROTO_CODE = 119;
    public const byte COM_PROTO_SYMM_KEY = 50;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MSG_HDR
    {
        public byte code;
        public UInt16 len;
        public byte randKey;
        public byte checkSum;
    }

    // 설정 값
    public const int RECV_BUFFER_LENGTH = 1024;
}

public class NetBuffer
{
    private byte[] m_Buffer;
    //private int m_Capacity;
    private int m_Index;

    public int BufferedSize { get { return m_Buffer.Length; } }

    public NetBuffer(int buffSize)
    {
        //m_Capacity = buffSize;  
        m_Buffer = new byte[buffSize];
    }

    public bool Write(byte[] source, int length, int offset = 0)
    {
        if (m_Index + length > m_Buffer.Length)
        {
            return false;
        }

        Array.Copy(source, offset, m_Buffer, m_Index, length);
        return true;
    }

    public bool Read(byte[] dest, int length)
    {
        if (m_Index < length)
        {
            return false;
        }

        Array.Copy(m_Buffer, dest, length);

        if (m_Index == length)
        {
            m_Index = 0;
        }
        else
        {
            byte[] temp = new byte[m_Index - length];
            Array.Copy(m_Buffer, length, temp, 0, m_Index - length);
            Array.Copy(temp, m_Buffer, m_Index - length);
            m_Index -= length;
        }
        return true;
    }
}

public class NetworkManager
{
    private TcpClient m_TcpClient = null;
    private NetworkStream m_Stream = null;

    private NetBuffer m_RecvBuffer = new NetBuffer(RECV_BUFFER_LENGTH);

    private System.Random m_RandKeyMaker = new System.Random();

    public NetworkManager(IPEndPoint ipEndPoint = null)
    {
        if (ipEndPoint == null)
        {
            m_TcpClient = new TcpClient();
        }
        else
        {
            m_TcpClient = new TcpClient(ipEndPoint);
            m_Stream = m_TcpClient.GetStream();
        }
    }

    public bool Connected { get { return m_TcpClient.Connected; } }

    public bool Connect()
    {
        if (!Connected)
        {
            try
            {
                m_TcpClient.Connect(IPAddress.Parse("127.0.0.1"), 7777);
                m_Stream = m_TcpClient.GetStream();
            }
            catch
            {
                return false;
            }
        }

        return true;
    }

    public void Disconnect()
    {
        if (Connected)
        {
            m_TcpClient.Close();    
        }
    }

    public void ClearRecvBuffer()
    {
        while (m_Stream.DataAvailable)
        {
            byte[] buffer = new byte[1024];
            m_Stream.Read(buffer, 0, buffer.Length);        
        }
    }

    public bool SendPacket<T>(T packet, bool encoding = true)
    {
        byte[] bytes = MessageToBytes(packet);

        if (encoding)
        {
            COM_PROTOCOL.MSG_HDR hdr = new COM_PROTOCOL.MSG_HDR();
            hdr.code = COM_PROTOCOL.COM_PROTO_CODE;
            hdr.len = (ushort)Marshal.SizeOf(typeof(T));
            hdr.randKey = GetRandKey();

            Encode(COM_PROTOCOL.COM_PROTO_SYMM_KEY, hdr.randKey, hdr.len, out hdr.checkSum, bytes);

            byte[] hdrPayload = new byte[Marshal.SizeOf(typeof(COM_PROTOCOL.MSG_HDR)) + Marshal.SizeOf(typeof(T))];

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(COM_PROTOCOL.MSG_HDR)));
            try
            {
                Marshal.StructureToPtr(hdr, ptr, false);
                Marshal.Copy(ptr, hdrPayload, 0, Marshal.SizeOf(typeof(MSG_HDR)));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            Buffer.BlockCopy(bytes, 0, hdrPayload, Marshal.SizeOf(typeof(MSG_HDR)), bytes.Length);

            try
            {
                m_Stream.Write(hdrPayload, 0, hdrPayload.Length);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        else
        {
            try
            {
                m_Stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }

        return true;
    }

    public bool ReceiveDataAvailable()
    {
        return m_Stream.DataAvailable;
    }

    public byte[] ReceivePacket(int waitSec = 5)
    {
        byte[] hdrbytes = null;
        COM_PROTOCOL.MSG_HDR hdr = null;
        do
        {
            if (m_TcpClient.Available >= Marshal.SizeOf<COM_PROTOCOL.MSG_HDR>())
            {
                hdrbytes = new byte[Marshal.SizeOf<COM_PROTOCOL.MSG_HDR>()];
                m_Stream.Read(hdrbytes, 0, hdrbytes.Length);
                hdr = BytesToMessage<COM_PROTOCOL.MSG_HDR>(hdrbytes);
                break;
            }
            Thread.Sleep(1000);
        } while(waitSec-- > 0);

        if(hdr == null)
        {
            return null;
        }
        else
        {
            if (m_TcpClient.Available < hdr.len)
            {
                m_RecvBuffer.Write(hdrbytes, hdrbytes.Length);
                return null;
            }
            else
            {
                byte[] payload = new byte[hdr.len];
                m_Stream.Read(payload, 0, payload.Length);

                if (!Decode(COM_PROTOCOL.COM_PROTO_SYMM_KEY, hdr.randKey, hdr.len, hdr.checkSum, payload))
                {
                    Debugger.Break();
                    return null;
                }

                return payload;
            }
        }
    }

    public bool ReceivePacket<T>(out T recvMessage, bool decoding = true, int waitSec = 5)
    {
        int recvSize = m_TcpClient.Available;

        for (int i = 0; i < waitSec; i++)
        {
            recvSize = m_TcpClient.Available;
            if (decoding)
            {
                if (recvSize >= Marshal.SizeOf<COM_PROTOCOL.MSG_HDR>() + Marshal.SizeOf(typeof(T)))
                {
                    break;
                }
            }
            else
            {
                if (recvSize < Marshal.SizeOf(typeof(T)))
                {
                    break;
                }
            }

            Thread.Sleep(1000);
        }

        if (decoding)
        {
            if (recvSize < Marshal.SizeOf<COM_PROTOCOL.MSG_HDR>() + Marshal.SizeOf(typeof(T)))
            {
                recvMessage = default(T);
                return false;
            }
            else
            {
                byte[] hdrbytes = new byte[Marshal.SizeOf<COM_PROTOCOL.MSG_HDR>()];
                m_Stream.Read(hdrbytes, 0, hdrbytes.Length);
                COM_PROTOCOL.MSG_HDR hdr = BytesToMessage<COM_PROTOCOL.MSG_HDR>(hdrbytes);

                byte[] payload = new byte[Marshal.SizeOf(typeof(T))];
                m_Stream.Read(payload, 0, payload.Length);

                if (!Decode(COM_PROTOCOL.COM_PROTO_SYMM_KEY, hdr.randKey, hdr.len, hdr.checkSum, payload))
                {
                    Debugger.Break();
                    recvMessage = default(T);
                    return false;
                }

                recvMessage = BytesToMessage<T>(payload);
            }
        }
        else
        {
            if (recvSize < Marshal.SizeOf(typeof(T)))
            {
                recvMessage = default(T);
                return false;
            }
            else
            {
                byte[] payload = new byte[Marshal.SizeOf(typeof(T))];
                m_Stream.Read(payload, 0, payload.Length);
                recvMessage = BytesToMessage<T>(payload);
            }
        }

        return true;
    }

    /*
    public bool ReceivMsgHdr(out COM_PROTOCOL.MSG_HDR hdr, int waitSec = 5)
    {
        int recvSize = m_TcpClient.Available;

        for (int i = 0; i < waitSec; i++)
        {
            recvSize = m_TcpClient.Available;

            if (recvSize >= Marshal.SizeOf<COM_PROTOCOL.MSG_HDR>())
            {
                break;
            }

            Thread.Sleep(1000);
        }

        if (recvSize < Marshal.SizeOf<COM_PROTOCOL.MSG_HDR>())
        {
            hdr = default(COM_PROTOCOL.MSG_HDR);
            return false;
        }

        byte[] hdrbytes = new byte[Marshal.SizeOf<COM_PROTOCOL.MSG_HDR>()];
        m_Stream.Read(hdrbytes, 0, hdrbytes.Length);
        hdr = BytesToStruct<COM_PROTOCOL.MSG_HDR>(hdrbytes);

        return true;
    }

    public bool ReceiveMsgBody<T>(COM_PROTOCOL.MSG_HDR hdr, out T recvMessage, int waitSec = 5)
    {
        int recvSize = m_TcpClient.Available;

        for (int i = 0; i < waitSec; i++)
        {
            recvSize = m_TcpClient.Available;

            if (recvSize >= Marshal.SizeOf(typeof(T)))
            {
                break;
            }
            Thread.Sleep(1000);
        }

        if (recvSize < Marshal.SizeOf(typeof(T)))
        {
            recvMessage = default(T);
            return false;
        }

        byte[] payload = new byte[Marshal.SizeOf(typeof(T))];
        m_Stream.Read(payload, 0, payload.Length);

        if (!Decode(COM_PROTOCOL.COM_PROTO_SYMM_KEY, hdr.randKey, hdr.len, hdr.checkSum, payload))
        {
            Debugger.Break();
            recvMessage = default(T);
            return false;
        }

        recvMessage = BytesToStruct<T>(payload);
        return true;
    }
    */

    private byte[] MessageToBytes<T>(T str)
    {
        int size = Marshal.SizeOf(str);
        byte[] arr = new byte[size];

        // Marshal.AllocHGlobal(size)
        // : 지정된 바이트 크기의 네이티브 메모리 할당
        //  반환되는 IntPtr은 할당된 메모리의 포인터를 나타냄
        //  이 포인터는 관리되지 않는 네이티브 메모리를 가리킴.
        //  주로 네이티브 코드와의 데이터 교환을 위해 사용
        IntPtr ptr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(str, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
        }
        catch (Exception ex)
        {
            Debugger.Break();
        }
        finally
        {
            // 할당받은 네이티브 메모리 해제
            Marshal.FreeHGlobal(ptr);
        }

        return arr;
    }
    private void MessageToBytes<T>(T str, byte[] dest)
    {
        int size = Marshal.SizeOf(str);

        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(str, ptr, false);
            Marshal.Copy(ptr, dest, 0, size);
        }
        catch (Exception ex)
        {
            Debugger.Break();
        }
        finally
        {
            // 할당받은 네이티브 메모리 해제
            Marshal.FreeHGlobal(ptr);
        }
    }

    public T BytesToMessage<T>(byte[] bytes)
    {
        T str = default(T);
        int size = Marshal.SizeOf<T>();
        IntPtr ptr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.Copy(bytes, 0, ptr, size);
            str = Marshal.PtrToStructure<T>(ptr);
        }
        catch (Exception ex)
        {
            Debugger.Break();
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return str;
    }

    private byte GetRandKey()
    {
        return (byte)(m_RandKeyMaker.Next(0, 255));
    }
    private void Encode(byte symmetricKey, byte randKey, ushort payloadLen, out byte checkSum, byte[] payloads)
    {
        byte payloadSum = 0;
        for (ushort i = 0; i < payloadLen; i++)
        {
            payloadSum += payloads[i];
            payloadSum = (byte)(payloadSum % 256);
        }
        byte Pb = (byte)(payloadSum ^ (randKey + 1));
        byte Eb = (byte)(Pb ^ (symmetricKey + 1));
        checkSum = Eb;

        for (ushort i = 1; i <= payloadLen; i++)
        {
            //byte Pn = payloads[i - 1] ^ (Pb + randKey + (byte)(i + 1));
            //byte En = Pn ^ (Eb + dfPACKET_KEY + (byte)(i + 1));
            byte Pn = (byte)(payloads[i - 1] ^ (Pb + randKey + i + 1));
            byte En = (byte)(Pn ^ (Eb + symmetricKey + i + 1));

            payloads[i - 1] = En;

            Pb = Pn;
            Eb = En;
        }
    }

    private bool Decode(byte symmetricKey, byte randKey, ushort payloadLen, byte checkSum, byte[] payloads)
    {
        byte Pb = (byte)(checkSum ^ (symmetricKey + 1));
        byte payloadSum = (byte)(Pb ^ (randKey + 1));
        byte Eb = checkSum;
        byte Pn;
        byte Dn;
        byte payloadSumCmp = 0;

        for (ushort i = 1; i <= payloadLen; i++)
        {
            Pn = (byte)(payloads[i - 1] ^ (Eb + symmetricKey + i + 1));
            Dn = (byte)(Pn ^ (Pb + randKey + i + 1));

            Pb = Pn;
            Eb = payloads[i - 1];
            payloads[i - 1] = Dn;
            payloadSumCmp += payloads[i - 1];
            payloadSumCmp = (byte)(payloadSumCmp % 256);
        }

        if (payloadSum != payloadSumCmp)
        {
            return false;
        }

        return true;
    }

    public enPacketType GetMsgTypeInBytes(byte[] payloads)
    {
        if (payloads == null)
            throw new ArgumentNullException(nameof(payloads));
        if (sizeof(ushort) > payloads.Length)
            throw new ArgumentOutOfRangeException(nameof(payloads));

        ushort usPacketType = BitConverter.ToUInt16(payloads, 0);
        return (enPacketType)usPacketType;
    }

    public bool CheckMsgType(ushort msgType, enPacketType expectedType, string debugLog)
    {
        if(msgType != (ushort)expectedType)
        {
            UnityEngine.Debug.Log(debugLog);
            return false;
        }
        
        return true;
    }

    public bool CheckReplyCode(ushort replyCode, enProtocolComReply expectedReplyCode, string debugLog)
    {
        if(replyCode != (ushort)expectedReplyCode)
        {
            UnityEngine.Debug.Log(debugLog);
            return false;
        }

        return true;
    }

    public void SetRequstMessage(MSG_COM_REQUEST requestMsg, enProtocolComRequest requestCode)
    {
        requestMsg.type = (ushort)enPacketType.COM_REQUSET;
        requestMsg.requestCode = (ushort)requestCode;   
    }
}