﻿using N_m3u8DL_RE.Common.Entity;
using N_m3u8DL_RE.Common.Enum;
using N_m3u8DL_RE.Common.Log;
using N_m3u8DL_RE.Common.Util;
using N_m3u8DL_RE.Parser.Config;
using N_m3u8DL_RE.Parser.Processor;
using N_m3u8DL_RE.Parser.Processor.HLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_m3u8DL_RE.Processor
{
    internal class DemoProcessor2 : KeyProcessor
    {
        public override bool CanProcess(ExtractorType extractorType, string method, string uriText, ParserConfig parserConfig)
        {
            return extractorType == ExtractorType.HLS  && parserConfig.Url.Contains("playertest.longtailvideo.com");
        }

        public override byte[]? Process(string method, string uriText, ParserConfig parserConfig)
        {
            Logger.InfoMarkUp($"[white on green]My Key Processor => {uriText}[/]");
            var key = new DefaultHLSKeyProcessor().Process(method, uriText, parserConfig);
            Logger.InfoMarkUp("[red]" + HexUtil.BytesToHex(key!, " ") + "[/]");
            return key;
        }
    }
}
