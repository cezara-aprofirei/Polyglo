import subprocess
import sys

def guess_word(description):
    # Updated prompt
    prompt = f"Guess the word from this description and give just the word: {description}"
    
    try:
        # Determine if shell is needed for cross-platform compatibility
        use_shell = sys.platform == "win32"
        
        # Run the Ollama model using the new gemma3:1b model
        result = subprocess.run(
            ["C:\\Users\\cezar\\AppData\\Local\\Programs\\Ollama\\ollama.exe", "run", "gemma3:1b", prompt],  # Switching model to gemma3:1b
            capture_output=True,
            text=True,
            shell=use_shell
        )

        # Check for errors
        if result.returncode != 0:
            print("Error:", result.stderr.strip())
            return
        
        # Print the raw output to inspect
        print(result.stdout)
        
        # Extract the response (assuming the response is on the last line)
        #response = result.stdout.strip().split("\n")[-1]
        #print("Model's Guess:", response)

    except Exception as e:
        print("Error:", str(e))

if __name__ == "__main__":
    if len(sys.argv) > 1:
        description = sys.argv[1]
        guess_word(description)
    else:
        print("Error: No description provided.")
