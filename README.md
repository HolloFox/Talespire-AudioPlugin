# Audio Plugin

This unofficial TaleSpire plugin adds custom ambient and/or music to the core Talespire atmosphere GUI.
Supports both local files and internet files.

This is a colaborative work between Lord Ashes and HolloFox. Lord Ashes did the initial code but most of it
was rewritten by Hollo to use the Atmosphere GUI. Instead of generating new plugin, it was decided to replace
Lord Ashes' plugin. Lord Ashes did a few improvement to Hollo's code for future expandability and added the
remote URL support.

This plugin, like all others, is free but if you want to donate, use: http://LordAshes.ca/TalespireDonate/Donate.php

## Change Log

```
2.0.1: Added duplicate audio file detection and warning
2.0.0: Added support for sharing remote library
2.0.0: Added support for remote requests
1.1.0: Rewritten to use Atmosphere GUI for selection
1.0.0: Initial release
```

## Install

Use R2ModMan or similar installer to install this plugin.

## Usage

### Content

The plugin will search for Ambient audio in any File Access Plugin friendly location with the following folder
structure:

``CustomData/Audio/Ambient``

The plugin will search for Music audio in any File Access Plugin friendly location with the following folder
structure:

``CustomData/Audio/Music``

Any files, with the extensions listed below, that is in either of these folders will be registered by this plugin
and will show up in the corresponding section of the Atmosphere GUI.

Supported extensions: ``AIF, MP3, OGG, WAV, WWW``

Extensions other than WWW are assoicated with local files where the content of the file contains the audio.
The WWW extension is a special case where the content of the file is a text link (URL) to an internet source for
the audio. 

For example, the file "One More Time.www" might contain the content "http://audiowebsite/music/OneMoreTime.mp3"

The name that shows up in the GUI is the file name with the extension stripped. As such the filename should be
meaningful so as to provide a good title in the menu. The file name can contain spaces.

### Remote Library Share

All ambient and music that is remote (uses the WWW functionality discussed above) can be shared with others
so that they don't need to have the same WWW files. There are two modes of sharing: useOnly and copyLocal.
Both the sender (typically GM) and the receiver (typically player) must agree to copyLocal otherwise useOnly
mode is uded.

To invoke a share of the remote library, press the corresponding keyboard shortcut (default RCTRL+A).

If either the GM, or the player, or both have selected useOnly then use only mode is used. In this mode the
remote library is sent and registered with the reciever for the session only. The GM will need to re-send the
remote library each session because the receiver does not retain knowledge of the remote library. This mode is
ideal when the GM want to limit the remote library to his own sessions but not have other players use the contents
in their own sessions. This mode is also idea for players who don't want the GM's audio content mixed into
their own collection. The receiver will have full access to the remote library during the session but once the
session ends, the remote library will be forgotten.

If both the Gm and player select copyLocal then copy local mode is used. In this mode not only will the remote
library be registered for the session but the system will also create the corresponding WWW files so that the
remote content is available when the session is reloaded and/or in other sessions.

### Remote Audio Request

Other plugins can trigger request of audio to play by writing to the org.hollofox.plugins.audio Asset Data key.
The content of the message should be the user, followed by an at-sign (@), followed by the audio category and
audio name. For example, ``LordAshes@Music/TavernSong01``

To send a request to all players, ommit the user name. For example, ``@Music/TavernSong01``

## Limitations

1. The remote audio requuets use their own audio track and thus are unaffected by the Atmosphere settings.
2. The remote audio requests cannot be turned off. They need to finish on their own.
