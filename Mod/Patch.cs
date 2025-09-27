using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly.WealthDisplayNoLogPatch
{
    public static class Patch
    {
        public static HarmonyX harmony = null;
        public static string harmonyId = "Lilly.";
        public static Type WealthDrawer = Type.GetType("WealthDisplayContinued.Interface.WealthDrawer, WealthDisplay(Continued)");
        public static FieldInfo lastPawnY = AccessTools.Field(WealthDrawer, "lastPawnY");

        public static void OnPatch(bool repatch = false)
        {
            if (repatch)
            {
                Unpatch();
            }
            if (harmony != null || !Settings.onPatch) return;
            harmony = new HarmonyX(harmonyId);
            try
            {
                harmony.PatchAll();
                MyLog.Message($"Patch <color=#00FF00FF>Succ</color>");
            }
            catch (System.Exception e)
            {
                MyLog.Error($"Patch Fail");
                MyLog.Error(e.ToString());
                MyLog.Error($"Patch Fail");
            }
            
        }

        public static void Unpatch()
        {
            MyLog.Message($"UnPatch");
            if (harmony == null) return;
            harmony.UnpatchSelf();
            harmony = null;
        }

        // 오류남. 수정 귀찬
        //[HarmonyPatch("WealthDisplayContinued.Interface.WealthDrawer, WealthDisplay(Continued)", "setLastPawnY")]
        //[HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MyLog.Message($"ST");

            var codeList = new List<CodeInstruction>(instructions);
            var logMethod = AccessTools.Method(typeof(Verse.Log), "Message", new[] { typeof(string) });

            for (int i = 0; i < codeList.Count; i++)
            {
                if (codeList[i].opcode == OpCodes.Call && codeList[i].operand as MethodInfo == logMethod)
                {
                    // Log.Message 호출 전후 코드 제거
                    int start = i;
                    while (start > 0 && codeList[start].opcode != OpCodes.Ldstr)
                        start--;

                    int end = i + 1;
                    codeList.RemoveRange(start, end - start);
                    MyLog.Message($"SUCC");
                    break;
                }
            }

            MyLog.Message($"ED");
            return codeList;
        }

        [HarmonyPatch("WealthDisplayContinued.Interface.WealthDrawer, WealthDisplay(Continued)", "setLastPawnY")]
        [HarmonyPrefix]
        public static bool Prefix(float pawnY, float size)
        {
            //if (WealthDrawer==null)
            //{
            //    MyLog.Error($"WealthDrawer is null");
            //    return true;
            //}
            //if (lastPawnY==null)
            //{
            //    MyLog.Error($"lastPawnY is null");
            //    return true;
            //}
            // 로그 출력 제거하고 값만 설정
            //WealthDisplayContinued.Interface.WealthDrawer.lastPawnY = pawnY + size;
            lastPawnY.SetValue(null, pawnY + size);

            // 원본 메서드 실행 막기
            return false;
        }


    }
}
