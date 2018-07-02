using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Yukarinette;
using System.Windows.Controls;

namespace YukarinetteMisakuraFilterPlugin
{
    public class MisakuraFilterPlugin : IYukarinetteFilterInterface
    {
        #region misakuraPatterns
        private static List<KeyValuePair<string, string>> misakuraPatterns = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("(気持|きも)ちいい", "ぎも゛ぢい゛い゛ぃ"),
            new KeyValuePair<string, string>("(大好|だいす)き", "らいしゅきいぃっ"),
            new KeyValuePair<string, string>("(ミルク|みるく|牛乳)", "ちんぽミルク"),
            new KeyValuePair<string, string>("お(願|ねが)い", "お願いぃぃぃっっっﾞ"),
            new KeyValuePair<string, string>("ぁ", "ぁぁ゛ぁ゛"),
            new KeyValuePair<string, string>("あ", "ぁあああ あぉ"),
            new KeyValuePair<string, string>("お", "おﾞぉおォおん"),
            new KeyValuePair<string, string>("ごきげんよう", "ごきげんよぉおお！んおっ！んおおーーっ！ "),
            new KeyValuePair<string, string>("ごきげんよう", "ごきげんみゃぁあ゛あ゛ぁ゛ぁぁあ！！"),
            new KeyValuePair<string, string>("こん(にち|ばん)[はわ]", "こん$1みゃぁあ゛あ゛ぁ゛ぁぁあ！！"),
            new KeyValuePair<string, string>("えて", "えてへぇええぇﾞ"),
            new KeyValuePair<string, string>("する", "するの"),
            new KeyValuePair<string, string>("します", "するの"),
            new KeyValuePair<string, string>("精液", "せーしっせーし でりゅぅ でちゃいましゅ みるく ちんぽみるく ふたなりみるく"),
            new KeyValuePair<string, string>("射精", "でちゃうっ れちゃうよぉおおﾞ"),
            new KeyValuePair<string, string>("(馬鹿|バカ|ばか)", "バカ！バカ！まんこ!!"),
            new KeyValuePair<string, string>("いい", "いぃぃっよぉおおﾞ"),
            new KeyValuePair<string, string>("[好す]き", "ちゅき"),
            new KeyValuePair<string, string>("して", "してぇぇぇぇ゛"),
            new KeyValuePair<string, string>("行く", "んはっ イっぐぅぅぅふうぅ"),
            new KeyValuePair<string, string>("いく", "イっくぅぅふぅん"),
            new KeyValuePair<string, string>("イク", "イッちゃううぅん"),
            new KeyValuePair<string, string>("駄目", "らめにゃのぉぉぉ゛"),
            new KeyValuePair<string, string>("ダメ", "んぉほぉぉォォ　らめぇ"),
            new KeyValuePair<string, string>("だめ", "らめぇぇ"),
            new KeyValuePair<string, string>("ちゃん", "ちゃぁん"),
            new KeyValuePair<string, string>("(おい|美味)しい", "$1ひぃ…れしゅぅ"),
            new KeyValuePair<string, string>("(た|る|ない)([。、　 ・…!?！？」\n\r\x00])", "$1の$2"),
            new KeyValuePair<string, string>("さい([。、　 ・…!?！？」\n\r\x00])", "さいなの$1"),
            new KeyValuePair<string, string>("よ([。、　 ・…!?！？」\n\r\x00])", "よお゛お゛お゛ぉ$1"),
            new KeyValuePair<string, string>("もう", "んもぉ゛お゛お゛ぉぉ"),
            new KeyValuePair<string, string>("(い|入)れて", "いれてえぇぇぇえ"),
            new KeyValuePair<string, string>("(気持|きも)ちいい", "きも゛ぢい゛～っ"),
            new KeyValuePair<string, string>("(がんば|頑張)る", "尻穴ちんぽしごき$1るぅ!!!"),
            new KeyValuePair<string, string>("出る", "でちゃうっ れちゃうよぉおおﾞ"),
            new KeyValuePair<string, string>("でる", "でっ…でるぅでるうぅうぅ"),
            new KeyValuePair<string, string>("です", "れしゅぅぅぅ"),
            new KeyValuePair<string, string>("ます", "ましゅぅぅぅ"),
            new KeyValuePair<string, string>("はい", "はひぃ"),
            new KeyValuePair<string, string>("スゴイ", "スゴぉッ!!"),
            new KeyValuePair<string, string>("(すご|凄)い", "しゅごいのょぉぉぅ"),
            new KeyValuePair<string, string>("だ", "ら"),
            new KeyValuePair<string, string>("さ", "しゃ"),
            new KeyValuePair<string, string>("な", "にゃ"),
            new KeyValuePair<string, string>("つ", "ちゅ"),
            new KeyValuePair<string, string>("ちゃ", "ひゃ"),
            new KeyValuePair<string, string>("じゃ", "に゛ゃ"),
            new KeyValuePair<string, string>("ほ", "ほお゛お゛っ"),
            new KeyValuePair<string, string>("で", "れ"),
            new KeyValuePair<string, string>("す", "しゅ"),
            new KeyValuePair<string, string>("ざ", "じゃ"),
            new KeyValuePair<string, string>("い", "いぃ"),
            new KeyValuePair<string, string>("の", "のぉおお"),
        };
        #endregion

        private SettingPanel _SettingPanel;
        private Random r = new System.Random();

        public override string Name => "みさくら語コンバーター with ゆかりねっと";

        public MisakuraFilterPlugin()
            : base()
        {
            _SettingPanel = new SettingPanel();
        }

        public override UserControl GetSettingUserControl()
        {
            return _SettingPanel;
        }

        public override YukarinetteFilterPluginResult Filtering(string text, YukarinetteWordDetailData[] words)
        {
            var ret = new YukarinetteFilterPluginResult();
            ret.Text = Convert2Misakura(text);

            return ret;
        }

        public string Convert2Misakura(string str)
        {
            string ret = str;
            var coverted = string.Copy(str);
            int probability = (int)_SettingPanel.slider.Value;
            if (ApproveConerting(probability))
            {
                foreach (var p in misakuraPatterns)
                {
                    coverted = Regex.Replace(coverted, p.Key, p.Value);
                }
                ret = coverted;
            }

            return ret;
        }

        public bool ApproveConerting(int probability)
        {
            var rval = r.Next(100);
            return rval < probability;
        }
    }
}
