import streamlit as st
from openai import OpenAI
import requests

# タイトルを表示する。
st.title("Simple agent client")

if "Clear" not in st.session_state:
    st.session_state.Clear = False

if "messages" not in st.session_state:
    st.session_state.messages = []

if "called_agent" not in st.session_state:
    st.session_state.called_agent = []

url = "http://localhost:7133/api/invoke/sync"

for idx, message in enumerate(st.session_state.messages):
    if not message["role"] == "system":
        if message["role"] == "user":
            with st.chat_message(message["role"], avatar="😊"):
                st.markdown(message["content"])
        elif message["role"] == "assistant":
            with st.chat_message(message["role"], avatar="🤖"):
                st.markdown(f"""
                            {message["content"]}
                            
                            使用されたエージェント:{st.session_state.called_agent[idx]}
                            """)

if prompt := st.chat_input("ここに入力"):
    st.session_state.messages.append({"role": "user", "content": prompt})
    st.session_state.called_agent.append("")
    with st.chat_message("user", avatar="😊"):
        st.markdown(prompt)

    with st.chat_message("assistant", avatar="🤖"):
        message_placeholder = st.empty()
        
        # APIにPOSTリクエストを送信
        response = requests.post(url, json={"messages": st.session_state.messages})
        response_data = response.json()
        response_content = response_data["content"]
        called_agents  = ", ".join(response_data["caledAgentNames"])
        
        message_placeholder.markdown(f"""
                                     {response_content}
                                     
                                     使用されたエージェント:{called_agents}
                                     """)
        st.session_state.messages.append({"role": "assistant", "content": response_content})
        st.session_state.called_agent.append(called_agents)
        st.session_state.Clear = True

if st.session_state.Clear:
    if st.button('clear chat history'):
        st.session_state.messages = []
        st.session_state.called_agent = []
        full_response = ""
        st.session_state.Clear = False 
        st.rerun()