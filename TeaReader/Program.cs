using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TeaReader
{
	class TEA
	{
		public byte[] comments = new byte[0x340];
		public int i1;
		public byte[] unk1 = new byte[4];
		public int i2;
		public byte[] unk2 = new byte[4+16];
		public byte[] id = new byte[4];
		public int i3;
		public float f1, f2, f3;
	}

	class SubTEA
	{
		public byte[] unk3 = new byte[16];
		public int i4, i5, u6, i7;
		public byte[] clipName = new byte[64];
		public float f4;
		public byte[] unk35 = new byte[8];
		public float f5, f6, f7, f8;
		public byte[] unk4 = new byte[4];
		public int i8;
		public byte[] unk5 = new byte[8];
		public int i9, i10, i11;
		public float f9;
	}

	class Program
	{
		private static Encoding ShiftJIS = Encoding.GetEncoding("Shift-JIS");
		private static StreamWriter Writer = null;

		static void Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Console.Write("Usage: TeaReader [-l {log file}] [{decrypted .tea file}]*");
				return;
			}

			bool next_file_is_logfile = false;
			foreach (string file in args)
			{
				if (next_file_is_logfile)
				{
					FileStream stream = new FileStream(file, FileMode.Create);
					Writer = new StreamWriter(stream, ShiftJIS);
					next_file_is_logfile = false;
				}
				else if (file == "-l")
				{
					next_file_is_logfile = true;
				}
				else
				{
					ShowTeaFile(file);
				}
			}
			if (Writer != null)
			{
				Writer.Close();
			}
		}

		private static void ShowTeaFile(string file)
		{
			if (!File.Exists(file))
				return;

			byte[] buffer;
			using (FileStream fs = File.OpenRead(file))
			{
				buffer = new byte[fs.Length];
				fs.Read(buffer, 0, (int)fs.Length);
			}

			String output = Path.GetFileName(file) + " len=" + buffer.Length;
			Console.WriteLine(output);
			if (Writer != null)
				Writer.WriteLine(output);

			TEA tea = new TEA();
			int bufIdx = 0;
			Array.Copy(buffer, tea.comments, tea.comments.Length);
			bufIdx += tea.comments.Length;
			tea.i1 = BitConverter.ToInt32(buffer, bufIdx);
			bufIdx += sizeof(int);
			bufIdx += tea.unk1.Length;
			tea.i2 = BitConverter.ToInt32(buffer, bufIdx);
			bufIdx += sizeof(int);
			bufIdx += tea.unk2.Length;
			Array.Copy(buffer, bufIdx, tea.id, 0, tea.id.Length);
			bufIdx += tea.id.Length;
			tea.i3 = BitConverter.ToInt32(buffer, bufIdx);
			bufIdx += sizeof(int);
			tea.f1 = BitConverter.ToSingle(buffer, bufIdx);
			bufIdx += sizeof(float);
			tea.f2 = BitConverter.ToSingle(buffer, bufIdx);
			bufIdx += sizeof(float);
			tea.f3 = BitConverter.ToSingle(buffer, bufIdx);
			bufIdx += sizeof(float);

			output = "i" + tea.i1 + " i" + tea.i2 + 
				String.Format(" ID: {0:X2}-{1:X2}-{2:X2}-{3:X2}", tea.id[0], tea.id[1], tea.id[2], tea.id[3]) +
				" i" + tea.i3 + " " + tea.f1 + " " + tea.f2 + " " + tea.f3;
			Console.WriteLine(output);
			if (Writer != null)
				Writer.WriteLine(output);

			SubTEA sub = new SubTEA();
			bufIdx = ShowSubTea(sub, buffer, bufIdx);

			output = "Stopped at x" + bufIdx.ToString("X4") + " rest " + (buffer.Length - bufIdx) + " bytes";
			Console.WriteLine(output);
			if (Writer != null)
				Writer.WriteLine(output);
		}

		private static int ShowSubTea(SubTEA sub, byte[] buffer, int bufIdx)
		{
			for (int i = 0; i < 1000; i++)
			{
				Array.Copy(buffer, bufIdx, sub.unk3, 0, sub.unk3.Length);
				bufIdx += sub.unk3.Length;
				sub.i4 = BitConverter.ToInt32(buffer, bufIdx);
				bufIdx += sizeof(int);
				sub.i5 = BitConverter.ToInt32(buffer, bufIdx);
				bufIdx += sizeof(int);
				sub.u6 = BitConverter.ToInt32(buffer, bufIdx);
				bufIdx += sizeof(int);
				sub.i7 = BitConverter.ToInt32(buffer, bufIdx);
				bufIdx += sizeof(int);
				Array.Copy(buffer, bufIdx, sub.clipName, 0, sub.clipName.Length);
				bufIdx += sub.clipName.Length;
				sub.f4 = BitConverter.ToSingle(buffer, bufIdx);
				bufIdx += sizeof(float);
				Array.Copy(buffer, bufIdx, sub.unk35, 0, sub.unk35.Length);
				bufIdx += sub.unk35.Length;
				sub.f5 = BitConverter.ToSingle(buffer, bufIdx);
				bufIdx += sizeof(float);
				sub.f6 = BitConverter.ToSingle(buffer, bufIdx);
				bufIdx += sizeof(float);
				sub.f7 = BitConverter.ToSingle(buffer, bufIdx);
				bufIdx += sizeof(float);
				sub.f8 = BitConverter.ToSingle(buffer, bufIdx);
				bufIdx += sizeof(float);
				Array.Copy(buffer, bufIdx, sub.unk4, 0, sub.unk4.Length);
				bufIdx += sub.unk4.Length;
				sub.i8 = BitConverter.ToInt32(buffer, bufIdx);
				bufIdx += sizeof(int);
				Array.Copy(buffer, bufIdx, sub.unk5, 0, sub.unk5.Length);
				bufIdx += sub.unk5.Length;
				sub.i9 = BitConverter.ToInt32(buffer, bufIdx);
				bufIdx += sizeof(int);
				sub.i10 = BitConverter.ToInt32(buffer, bufIdx);
				bufIdx += sizeof(int);
				sub.i11 = BitConverter.ToInt32(buffer, bufIdx);
				bufIdx += sizeof(int);
				sub.f9 = BitConverter.ToSingle(buffer, bufIdx);
				bufIdx += sizeof(float);

				string unk3str = String.Empty;
				for (int u = 0; u < sub.unk3.Length; u++)
				{
					if (sub.unk3[u] != 0)
						unk3str += " " + u + ":" + sub.unk3[u];
				}
				string unk35str = String.Empty;
				for (int u = 0; u < sub.unk35.Length; u++)
				{
					if (sub.unk35[u] != 0)
						unk35str += " " + u + ":" + sub.unk35[u];
				}
				string unk4str = String.Empty;
				for (int u = 0; u < sub.unk4.Length; u++)
				{
					if (sub.unk4[u] != 0)
						unk4str += " " + u + ":" + sub.unk4[u];
				}
				string unk5str = String.Empty;
				for (int u = 0; u < sub.unk5.Length; u++)
				{
					if (sub.unk5[u] != 0)
						unk5str += " " + u + ":" + sub.unk5[u].ToString("X2");
				}
				int clipNameLength = 0;
				while (clipNameLength < sub.clipName.Length && sub.clipName[clipNameLength] != 0)
					clipNameLength++;
				String output = "   " + i.ToString("D4") + ":" +
					(unk3str.Length > 0 ? " unk3=(" + unk3str + ")" : "") +
					" i" + sub.i4 + " i" + sub.i5 + (sub.u6 != 0 ? " UNK" + sub.u6 : "") + " i" + sub.i7 +
					" <" + ShiftJIS.GetString(sub.clipName, 0, clipNameLength) +
					"> " + sub.f4 +
					(unk35str.Length > 0 ? " unk35=(" + unk35str + ")" : "") +
					" " + sub.f5 + " " + sub.f6 + " " + sub.f7 + " " + sub.f8 +
					(unk4str.Length > 0 ? " unk4=(" + unk4str + ")" : "") +
					" i" + sub.i8 +
					(unk5str.Length > 0 ? " unk5=(" + unk5str + ")" : "") +
					" i" + sub.i9 + " i" + sub.i10 + " i" + sub.i11 + " " + sub.f9;
				Console.WriteLine(output);
				if (Writer != null)
					Writer.WriteLine(output);

				if (sub.f5 == 0f)
					break;
			}
			return bufIdx;
		}
	}
}
