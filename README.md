Collection of code for Project ArkC
===================================

**Noah**, on behalf of Project ArkC Team. **September, 2016**.

---

This repository contains all primary codes developed in Project ArkC from different repositories for reference. All the code is licensed under GNU GPLv2 unless another license is included in that project folder.

Since June, 2016, the development of Project ArkC has been **suspended and no longer supported**. While major functions of ArkC have been finished, **bugs and other design flaws may exist in the code**.

---

#### What did Project ArkC try to do? Why?

Project ArkC tried different approaches to design open source proxy software (or in general, circumvention solutions) in order to boost proxy performance in People's Republic of China, where heavy censorship limits residents' access to information and freedom of speech. In particular, Project ArkC aimed to provide a satisfactory proxy solution with limited or no financial resource commitment.

The following consensus, shared by Project ArkC developers, could explain the goal of Project ArkC:

>> **Access to information generated overseas should be democratized, not only among intellectuals but for students and ordinary middle-class Chinese.**

It is typical among students and ordinary middle-class Chinese people to be reluctant to pay for VPNs or proxy services for overseas information, so there must be free services so that access to information is genuinely democratized. Such services must also be stable enough for continual use, otherwise users have to spend much time searching for other solutions.

>> **Internet connectivity should be provided with neutrality. Proxy or VPN service should be politically neutral, so that the service providers ideally should be able to support their service without external fundings.**

A free proxy or VPN service should not be supported based on advertisements or fundings, because advertisements or fundings might affect the neutrality of the proxy or VPN service. Therefore, the technology used to support the free proxy or VPN service must be affordable.

>> **Internet connectivity should not be accomplished at the cost of privacy or security.**

The technology / software to support a free proxy or VPN service should be subject to public audit so that a service does not intercept online communication of its users. Ideally, all codes and designs related to the free proxy or VPN service should be open source.

**For the above reasons, Project ArkC aims to provide open source proxy solutions with little deployment and operational costs.**

#### What has this project achieved in technology?

The project provides proxy solutions with following features:

* TCP Proxy connections can be initiated from servers to clients to avoid Deep Packet Inspection (DPI)

* Use Google App Engine (within its free allowance) as proxy servers, as long as **UrlFetch to China** is not blocked

* Multiplexing in TCP proxy connections to obfuscate bandwidth characteristics

* Basic NAT-traversal functions

#### Why has this project been suspended?

Several factors contribute to the suspension.

* Changes in the political environment in China to suppress the civil society since late 2015 exposed developers to potential personal threats.

* Moves from major Chinese ISPs (provide end users with Internet service behind NAT) limit the application of the project from the technology aspect.

* Other personal concerns of team members
 
#### Who has contributed to this project, apart from the authors of the open source modules used?

Pesudo-names are used during development of this project for the safety of team members, as the Chinese government has persecuted developers of several projects with similar goals.

Primary developers of Project ArkC are **Noah (Y. Yang), Teba (Y. Lu), Ddeerreekk (Y. Yang)**. However, at this moment, the copyright of any code in Project ArkC, along with liabilities, belongs to **ArkC Technology Inc**. ArkC Technology Inc is an entity founded in the United States of America and subject to its law.
