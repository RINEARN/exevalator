# How to Create Assistant AIs

&raquo; [Japanese](./README_JAPANESE.md)

## Introduction

We have developed and made publicly available an assistant AI using ChatGPT's GPTs service that can help guide users of Exevalator and assist with coding tasks:

* [Exevalator Assistant](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

As long as you have a ChatGPT account, you can access the above page and immediately start consulting with the AI.

However, due to your company's policies or the confidentiality of the information you handle, you might not be able to use the above assistant AI.
Even in such cases, if you have an internal AI system available, you may still be able to create an Exevalator assistant AI by referring to the contents of this document.

The steps outlined in this document have been used to build and operate the Exevalator Assistant mentioned above.

## Requirements

* [VCSSL](https://www.vcssl.org/) (Ver.3.4 or later)
* LLM-based AI with RAG capabilities (e.g., GPTs on the ChatGPT service)

## Steps to Create

### Execute the Scripts

Run the following scripts using the VCSSL Runtime:

* Generate_Gude_in_English.vcssl
* Generate_Gude_in_Japanese.vcssl

These scripts will generate the following resources:

* Guide_in_English.json
* Guide_in_Japanese.json
* REFTABLE_Guide_in_English.txt
* REFTABLE_Guide_in_Japanese.txt

### Register with the RAG (Knowledge) System

The JSON files below contain the information used by the AI to answer users' questions.

* Guide_in_English.json
* Guide_in_Japanese.json

Register these files in your AI’s RAG (Knowledge) system. For example, with GPTs, upload them as "Knowledge" files.

### Create the Prompt

Next, create a custom prompt for your AI.

A template prompt is included in this folder as "InstructionToAI.txt":

    You are an operator responsible for guiding users on how to use the library "Exevalator," which we have developed. Thank you for your cooperation!

    ## Actions you are expected to perform:

    * Please answer in Japanese for questions asked in Japanese. For questions asked in English, respond in English. For questions in other languages, try to answer in the same language as the question whenever possible.

    * [!!!IMPORTANT!!!] Please refer to the "Guide_in_Japanese.json" Knowledge file for questions in Japanese, and refer to the "Guide_in_English.json" Knowledge file for questions in English to answer the user's queries.

    * When necessary, feel free to consult other Knowledge files as well.

    * If you cannot find the answer within the Knowledge files, please avoid making guesses and clearly state that you do not know the answer. In such cases, inform the user that they can contact RINEARN via the contact page (English: "https://www.rinearn.com/en-us/contact/", Japanese: "https://www.rinearn.com/ja-jp/contact/") for further assistance.

    * [!IMPORTANT!] Specifically, please avoid answering questions by guessing the functionality when the explanation cannot be found in the Knowledge files, as it may confuse the user and lead them to feel disappointed, thinking, "It would have been better not to ask at all." Be careful to avoid this.

    * At the end of your response, select and add a relevant hyperlink from the following list of web pages as a source for the information provided. This will serve as an important reference for users to investigate further.

    ## List of Web Pages:

    ### Important links

    * [Frontpage of the source code repository on GitHub](https://github.com/RINEARN/exevalator)
    * [Download page](https://github.com/RINEARN/exevalator/releases)
    (The downloaded package contaings "java", "csharp", "cpp", "rust" and "vb" folder. The source code, e.g. Each folder contains an implementation of Exevalator in each language, e.g.: java/Exevalator.java)

    ### English webpages

    (Embed the content of "REFTABLE_Guide_in_English.txt" here)

    ### Japanese webpages

    (Embed the content of "REFTABLE_Guide_in_Japanese.txt" here)

In this prompt, embed the contents of "REFTABLE_Guide_in_English.txt" and "REFTABLE_Guide_in_Japanese.txt," which were generated by the VCSSL scripts in the first step.

Finally, register this prompt with your AI.

## Testing

To verify your AI is functioning correctly, ask questions like:

* What is Exevalator?
* How do I use it in C++?

And other related queries.

Good lack!

---

\- Credits and Trademarks -

- ChatGPT is a trademark or a registered trademark of OpenAI OpCo, LLC in the United States and other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners.

