"""Compile staged Unity sources and run package/planner checks without launching Unity."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import wave

import stage_runtime

HERE = Path(__file__).resolve().parent
ROOT = stage_runtime.ROOT
UNITY = ROOT / "unity/StorytellingVR"
LEVEL = ROOT / stage_runtime.LEVEL
FATHER = UNITY / "Assets/_Project/Audio/NPCVoices/Level1/FatherPenteadoV2/FatherVoiceBank.json"
EDITOR = Path(r"E:\Unity\Editor\6000.3.10f1\Editor\Data")


def run(args, cwd=ROOT):
    result = subprocess.run([str(a) for a in args], cwd=cwd, text=True, encoding="utf-8", errors="replace",
                            stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    if result.returncode:
        raise RuntimeError(result.stdout)
    if result.stdout:
        print(result.stdout.strip(), flush=True)


def create_fixture(root, character, speaker):
    root.mkdir()
    (root / "audio").mkdir()
    clips = []
    records = []

    def add(clip_id, text):
        relative = f"audio/{clip_id}.wav"
        path = root / relative
        with wave.open(str(path), "wb") as wav:
            wav.setparams((1, 3, 44100, 0, "NONE", "not compressed"))
            wav.writeframes(b"\x00\x10\x00\x00\x10\x00")
        digest = hashlib.sha256(path.read_bytes()).hexdigest()
        clips.append(dict(clipId=clip_id, text=text, relativeWavPath=relative, sha256=digest))
        records.append(dict(id=clip_id, speaker=speaker, spoken_text=text, wav_path=relative,
                            wav_sha256=digest, status="VALIDATED", sample_rate=44100, channels=1))
        return clip_id

    def pair(key, clip_id=None, text=None):
        if clip_id is None:
            clip_id = key.replace(" ", "_")
        return dict(key=key, id=add(clip_id, text or key))

    digits = "zero one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seventeen eighteen nineteen"
    numbers = [pair(w) for w in (digits + " twenty thirty forty fifty sixty seventy eighty ninety hundred and thousand million billion").split()]
    phrases = [dict(key="1", id="one"), pair("234", "whole_234", "two hundred and thirty-four"), pair("999", "whole_999", "nine hundred and ninety-nine")]
    quantity = [pair(q, clip_id) for q,clip_id in (("one Seer","one_seer"),("two Seers","quantity_pair"),("one Veesai","one_veesai"),("two Veesai","two_veesai"))]
    spices = [pair(s) for s in ("pepper","cardamom","cinnamon")]
    descriptors = [dict(key=s, id=add(s+"Desc", t), text=t) for s,t in (("pepper","black pepper"),("cardamom","green cardamom"),("cinnamon","cinnamon bark"))]
    units = [pair(u) for u in "Palam Palams Seer Seers Veesai Manangu Manangus Bahar Bahars".split()]
    start = add("start","At")
    plural = add("plural","varahas for")
    singular = add("singular","varaha for")
    of = add("of","of")
    hold = add("hold","I hold.")
    lot = add("lot","this lot")
    ops = [dict(kind="CLIP", id=start, sourcePiece="At "), dict(kind="VALUE", name="price"),
           dict(kind="CURRENCY_CLIP", id=plural, singularId=singular, priceSlot="price", sourcePiece=" varahas for "),
           dict(kind="VALUE", name="quantityLabel"), dict(kind="CLIP", id=of, sourcePiece=" of "),
           dict(kind="VALUE", name="spiceName"), dict(kind="PUNCTUATION", text=".")]
    source = "At {price} varahas for {quantityLabel} of {spiceName}."
    templates = [
        dict(id="Offer", scenario="BuyerCounterFirst", category="AUTHORED", sourceTemplate=source, spokenTemplate=source, operations=ops),
        dict(id="Hold", scenario="BuyerHoldsFirm", category="AUTHORED", sourceTemplate="I hold.", spokenTemplate="I hold.",
             operations=[dict(kind="CLIP", id=hold, sourcePiece="I hold.")]),
    ]
    replacements = dict(sourcePiece="source_piece", singularId="singular_id", priceSlot="price_slot")
    manifest = [dict(id=t["id"], scenario=t["scenario"], category=t["category"], source_expression_template=t["sourceTemplate"],
                     spoken_template=t["spokenTemplate"],
                     operations=[{replacements.get(k,k):v for k,v in op.items()} for op in t["operations"]]) for t in templates]
    bank = dict(characterId=character, sampleRate=44100, templates=templates, numbers=numbers, numberPhrases=phrases,
                quantityPhrases=quantity, spices=spices, descriptors=descriptors, units=units, quantityFallback=lot,
                holdTemplateId="Hold", aliases=[], processing=dict(assemblyVersion="indian-hysteresis-zero-number-v1", defaultGapMs=60))
    data = {
        "UnityVoiceBank.json": bank, "UnityVoiceClips.json": clips,
        "mapping_manifest.json": dict(version=1, character_id=character, voice=dict(character_id=character, speaker=speaker),
                                      recordings=records, templates=manifest, missing_template_ids=[]),
        "coverage_report.json": dict(character_id=character, speaker=speaker, ready_for_import_review=True, required=len(clips),
                                     wav_count=len(clips), mapping_count=2, required_mapping_count=2, missing_vocabulary=[]),
        "production_validation.json": dict(character=character, speaker=speaker, production_complete=True, tested_signals=len(clips),
                                            mapping_count=2, missing_assets=[], declared_missing_vocabulary=[]),
    }
    for name, value in data.items():
        (root / name).write_text(json.dumps(value), encoding="utf-8")


def main():
    csc = EDITOR / "DotNetSdkRoslyn/csc.dll"
    mono = EDITOR / "MonoBleedingEdge/bin/mono.exe"
    net = EDITOR / "MonoBleedingEdge/lib/mono/4.8-api"
    command = ["dotnet", "exec", "--roll-forward", "Major", csc, "-nologo"]
    with tempfile.TemporaryDirectory(prefix="AnubhavaIndianVoiceChecks_") as directory:
        temp = Path(directory)
        runtime, chat = (temp / "NpcVoicePresentation.cs"), (temp / "ChatManager.cs")
        runtime.write_text(stage_runtime.planner((LEVEL / runtime.name).read_text(encoding="utf-8-sig")), encoding="utf-8")
        chat.write_text(stage_runtime.chat((LEVEL / chat.name).read_text(encoding="utf-8-sig")), encoding="utf-8")
        bee = UNITY / "Library/Bee/artifacts"
        editor_rsp = max(bee.rglob("Assembly-CSharp-Editor.rsp"), key=lambda p: p.stat().st_mtime)
        flags = list(dict.fromkeys(line for line in editor_rsp.read_text(encoding="utf-8-sig").splitlines()
            if line.startswith(("-r:", "-define:", "-langversion:", "-nostdlib"))))
        flags = [f for f in flags if not f.endswith('/Assembly-CSharp.ref.dll"')]
        compile_files = [runtime, chat, LEVEL/"PreRecordedNpcTtsProvider.cs", LEVEL/"AudioManager.cs",
                         LEVEL/"Core/DialogueTableResponseProvider.cs", HERE/"IndianNpcVoicePackage.cs",
                         HERE/"IndianNpcVoicePackageImporter.cs", HERE/"IndianNpcVoiceIntegrationTests.cs"]
        rsp = temp/"compile.rsp"
        flags += ['-target:library', '-nowarn:0436,0649', f'-out:"{temp/"IndianTargetCompile.dll"}"',
                  f'-r:"{UNITY/"Library/ScriptAssemblies/Assembly-CSharp.dll"}"']
        rsp.write_text("\n".join(flags + [f'"{p}"' for p in compile_files]), encoding="utf-8")
        run(command + [f"@{rsp}"], UNITY)
        print("Unity API targeted compilation: passed.", flush=True)

        exe = temp/"IndianVoiceChecks.exe"
        refs = [net/n for n in ("mscorlib.dll","System.dll","System.Core.dll","System.Web.Extensions.dll")]
        run(command + ["-nostdlib+", "-nowarn:0649", f"-out:{exe}"] + [f"-r:{p}" for p in refs] +
            [runtime,HERE/"IndianNpcVoicePackage.cs",HERE/"StaticChecks.cs"])
        run([mono,exe,"legacy",FATHER])
        fixtures = []
        for character,speaker in (("lakshmi_amma","Swapna"),("chinnamma_naik","Priya"),
                                  ("abdul_rahman","Kabir"),("saraswati_chetti","Mary")):
            path = temp/character
            create_fixture(path,character,speaker)
            fixtures.append(path)
        run([mono,exe,"check",*fixtures])
        for mode, error in (("pending","incomplete"),("wrong-voice","identity"),("missing-clip","Missing processed WAV"),
                            ("bad-hash","checksum mismatch"),("path-traversal","Malformed clip index"),
                            ("unmapped","Segment differs from manifest")):
            path = temp/mode
            shutil.copytree(fixtures[0],path)
            if mode in ("pending","wrong-voice"):
                file = path/"coverage_report.json"
                data = json.loads(file.read_text(encoding="utf-8"))
                data.update({"pending":1} if mode=="pending" else {"speaker":"Mary"})
                file.write_text(json.dumps(data), encoding="utf-8")
            elif mode == "missing-clip":
                (path/"audio/one.wav").unlink()
            elif mode == "bad-hash":
                (path/"audio/one.wav").write_bytes(b"invalid take")
            elif mode == "path-traversal":
                file = path/"UnityVoiceClips.json"
                data = json.loads(file.read_text(encoding="utf-8"))
                data[0]["relativeWavPath"] = "../invalid.wav"
                file.write_text(json.dumps(data), encoding="utf-8")
            elif mode == "unmapped":
                file = path/"UnityVoiceBank.json"
                data = json.loads(file.read_text(encoding="utf-8"))
                data["templates"][0]["operations"][0]["id"] = "not-a-clip"
                file.write_text(json.dumps(data), encoding="utf-8")
            run([mono,exe,"reject",path,error])


if __name__ == "__main__":
    sys.dont_write_bytecode = True
    main()
