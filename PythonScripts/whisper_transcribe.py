import whisper
import sys
import json
import os
import warnings

warnings.filterwarnings("ignore", category=UserWarning, message="FP16 is not supported on CPU")

def main():
    if len(sys.argv) < 2:
        print(json.dumps({"error": "No audio file path provided"}))
        sys.exit(1)

    audio_path = sys.argv[1]

    if not os.path.isfile(audio_path):
        print(json.dumps({"error": f"Audio file not found at {audio_path}"}))
        sys.exit(1)

    try:
        model = whisper.load_model("base")
        result = model.transcribe(audio_path)
        print(json.dumps({ "text": result["text"] }))
    except Exception as e:
        print(json.dumps({"error": str(e)}))
        sys.exit(1)

if __name__ == "__main__":
    main()