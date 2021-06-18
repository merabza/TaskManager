საცდელი ამოცანა

ტექნოლოგიები:
ASP.Net Core Web Application, EntityFrameworkCore (Code First, InMemoryDatabase)
დავალება
Task Management ვებ აპლიკაცია, რომელშიც ავტორიზაციის გავლა უნდა შეეძლოს
ორი ტიპის მომხმარებელს: User და Support. User უნდა არეგისტირებდეს ტაკებს,
ხოლო Support ხედავდეს შემოსულ ტასკებს და ანიჭებს მას სტატუსებს.
Task იქმნება სამი მონაცემით: Title, Description და Priority. Task-ის სტატუსები უნდა
იყოს: New, InProgress, Done.

-----

სასურველია უფლებების შემოწმება ხდებოდეს არა როლების საშუალებით. ანუ კონკრეტულ შემთხვევაში შესაძლებელი უნდა იყოს მიმდინარე მომხმარებელს ამ კონკრეტულ მოქმედებაზე, ან ცვლილებაზე აქვს თუ არა უფლება.

გასატესტად შესაძლებელია შემდეგი მომხმარებლების გამოყენება:

მომხმარებელი: user@example.com
პაროლი: user
როლი: User. შეუძლია შექმნას ახალი ამოცანა. არ შეუძლია წაშლა, არ შეუძლია სტატუსის ცვლილება. თუმცა სხვა ველების რედაქტირება შეუძლია. არ შეუძლია სტატუსების ჩამონათვალში ახალი სტატუსის შექმნა. არ შეუძლია პრიორიტეტების ჩამონათვალში ახალი პრიორიტეტის შექმნა.

მომხმარებელი: support@example.com
პაროლი: support
როლი: Support. ჩვეულებრივი მომხმარებლისაგან იმით განსხვავდება, რომ დამატებით შეუძლია არსებულ ამოცანას შეუცვალოს სტატუსი

მომხმარებელი: admin@example.com
პაროლი: admin
როლი: Admin. ყველაფერი შეუძლია
