using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Wyszukiwarka {
    public class ZasobySerwera {
        public double ProcProcesora = 0;
        public double ProcDysku = 0;
        public double ProcRAM = 0;
        public int ZajeteRAM = 0;
    }

    public static class Wydajnosc {
        private static object sl = new object();
        private static ZasobySerwera zasoby = new ZasobySerwera();

        private static PerformanceCounter proc = new PerformanceCounter("Processor", "% Processor time", "_Total");
        private static PerformanceCounter dysk = new PerformanceCounter("Dysk fizyczny", "Czas dysku (%)", "_Total");

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        private struct PerformanceInformation {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        private static Int64 GetPhysicalAvailableMemoryInMiB() {
            PerformanceInformation pi = new PerformanceInformation();
            if(GetPerformanceInfo(out pi, Marshal.SizeOf(pi))) {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            } else {
                return -1;
            }
        }

        private static Int64 GetTotalMemoryInMiB() {
            PerformanceInformation pi = new PerformanceInformation();
            if(GetPerformanceInfo(out pi, Marshal.SizeOf(pi))) {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            } else {
                return -1;
            }
        }

        static Wydajnosc() {
            PobierzZasoby();
        }

        public static ZasobySerwera PobierzZasoby() {
            lock(sl) {
                ZasobySerwera zas = new ZasobySerwera();
                zas.ProcProcesora = proc.NextValue();
                if(zas.ProcProcesora <= 0.0 || zas.ProcProcesora > 100.0) zas.ProcProcesora = zasoby.ProcProcesora;
                zas.ProcDysku = dysk.NextValue();
                if(zas.ProcDysku <= 0.0 || zas.ProcDysku > 100.0) zas.ProcDysku = zasoby.ProcDysku;

                Int64 phav = GetPhysicalAvailableMemoryInMiB();
                Int64 tot = GetTotalMemoryInMiB();
                decimal percentFree = (phav / (decimal)tot) * 100;
                decimal percentOccupied = 100 - percentFree;

                zas.ZajeteRAM = (int)(tot - phav);
                zas.ProcRAM = (double)percentOccupied;
                zasoby = zas;
                return zas;
            }
        }
    }
}