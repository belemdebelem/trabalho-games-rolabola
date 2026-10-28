# Jogo de coleta (Unity)

Projeto didatico 3D: mova a esfera pela arena, colete os 12 cubos amarelos e veja a pontuacao e a mensagem de conclusao.

## Requisitos

- Unity Hub e Unity Editor 6.0 LTS (o projeto foi configurado para `6000.0.23f1`).
- Para usar o Unity 6 no Windows, Windows 10 21H1 ou posterior, ou Windows 11.
- Visual Studio Code com a extensao **Unity** (`visualstudiotoolsforunity.vstuc`).
- No Linux, instale tambem o suporte de editor C#/.NET indicado pela extensao do VS Code.

O Hub e o Editor sao programas separados: este repositorio contem o projeto, nao os instaladores. No Hub, instale a versao `6000.0.23f1` ou outra versao Unity 6 compativel. Para apenas abrir e jogar no Editor, nao e necessario instalar um modulo de build. O Unity Editor para Linux e oficialmente suportado em versoes selecionadas do Ubuntu; Fedora pode funcionar, mas nao esta entre as distribuicoes oficialmente listadas. O Hub instalado por Flatpak tambem pode precisar de permissao para acessar a pasta do projeto.

## Abrir e executar

1. No Unity Hub, escolha **Add** ou **Open** e selecione esta pasta (`JogoColetaUnity`).
2. Instale pelo Hub a versao `6000.0.23f1` ou selecione outra versao Unity 6 compativel. Aguarde o Editor importar os pacotes.
3. Na primeira abertura, o projeto gera a cena `Assets/Scenes/JogoColeta.unity`, os materiais e a configuracao inicial. Se necessario, use **Jogo de Coleta > Criar ou recriar cena** no menu do Editor.
4. Abra `Assets/Scenes/JogoColeta.unity` e pressione **Play**.
5. Para editar scripts, abra `JogoColetaUnity` no VS Code. Se o IntelliSense nao carregar, configure o VS Code como editor externo nas preferencias do Unity e gere novamente os arquivos de projeto.

## Controles

- **WASD** ou **setas**: movimentar a esfera.
- Recolha os 12 cubos para concluir a partida.

Scripts em `Assets/Scripts` implementam movimento, camera, coleta e pontuacao. A cena e montada pelo script de editor em `Assets/Editor/ProjectSceneSetup.cs`, para que os objetos e referencias fiquem configurados automaticamente ao abrir o projeto pela primeira vez.
