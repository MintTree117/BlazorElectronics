namespace BlazorElectronics.Server.Core;

public static class SeedData
{
    public static readonly string[] PRODUCT_TITLES = { "Book", "Software", "Video Game", "Movie/Tv", "Course" };
    public static readonly string[] PRODUCT_THUMBNAILS = { "BooksThumbnail.jpg", "SoftwareThumbnail.jpg", "VideoGamesThumbnail.jpg", "MoviesTvThumbnail.jpg", "CoursesThumbnail.jpg" };
    public static readonly string[][] PRODUCT_IMAGES =
    {
        new[] { "Book1.jpg", "Book2.jpg", "Book3.jpg", "Book4.jpg" },
        new[] { "Software1.jpg", "Software2.jpg", "Software3.jpg", "Software4.jpg" },
        new[] { "Game1.jpg", "Game2.jpg", "Game3.jpg", "Game4.jpg" },
        new[] { "Video1.jpg", "Video2.jpg", "Video3.jpg", "Video4.jpg" },
        new[] { "Course1.jpg", "Course2.jpg", "Course3.jpg", "Course4.jpg" },
    };

    public const int MAX_CATEGORIES = 3;
    
    public static readonly DateTime MIN_RELEASE_DATE = new ( 2000, 1, 1 );
    public static readonly DateTime MAX_RELEASE_DATE = DateTime.Now;
    
    public const int MAX_NUM_SOLD = 100000;
    public const double MIN_PRICE = 0.99;
    public const double MAX_PRICE = 9999.99;
    public const float MAX_RATING = 5.0f;

    public const int MAX_PAGE_LENGTH = 1000;
    public const int MAX_AUDIO_LENGTH = 1000;

    public const int MAX_RUNTIME = 500;
    public const int MAX_EPISODES = 1000;

    public const int MAX_COURSE_DURATION = 500;

    public const int MIN_FILE_SIZE = 1;
    public const int MAX_FILE_SIZE = 100000;

    public static readonly string[] BOOK_DESCR =
    {
        "Dive into the epic fantasy realm of 'Thrones of Destiny,' an Ebook that weaves a tale of power, betrayal, and magic. Follow the journey of a young hero who must navigate the treacherous alliances of the court to claim their rightful throne and save their kingdom from darkness.",
        "Unlock the mysteries of productivity with 'The Efficient You,' an Ebook designed to transform your work ethic. This guide offers practical advice on managing time, optimizing your workspace, and harnessing the power of habits to unlock your full potential.",
        "Explore the culinary delights of 'Flavors of the World' from the comfort of your home. This Ebook brings you a collection of recipes that takes you on a gastronomic tour, featuring dishes from the bustling streets of Bangkok to the quaint bistros of Paris."
    };
    public static readonly string[] SOFTWARE_DESCR =
    {
        "Introducing 'CodeMaster Pro,' a revolutionary software that streamlines the development process. It comes equipped with an intuitive interface, intelligent code completion, and robust debugging tools to enhance your coding efficiency.",
        "Discover 'BeatGuru,' the ultimate music production software for both aspiring and professional producers. With an extensive sound library, intuitive mixing console, and advanced mastering tools, create music that resonates with your unique style.",
        "Streamline your business with 'TaskFlow,' a task management software designed to optimize workflow. It features customizable boards, detailed analytics, and collaboration tools that help teams stay on track and deliver projects on time."
    };
    public static readonly string[] GAME_DESCR =
    {
        "Embark on an interstellar adventure with 'Galaxy Quest,' a video game that takes you on a thrilling journey across the universe. Command your own spacecraft, encounter alien species, and uncover cosmic mysteries in this expansive space exploration game.",
        "Step into the boots of a medieval knight in 'Castle Siege,' a strategy video game where you defend your stronghold against invading forces. Manage resources, lead your army, and strategize your way to victory in epic battles.",
        "Solve mind-bending puzzles in 'Quantum Conundrum,' a video game that challenges your perception of physics. Navigate through a mansion filled with inventive contraptions and alternate dimensions to uncover the secrets within."
    };
    public static readonly string[] MOVESTV_DESCR =
    {
        "Watch the heartwarming story unfold in 'The Last Sunset,' a movie about a retired pilot who embarks on a journey to reconnect with his estranged children, finding redemption and love along the way.",
        "Tune into 'Space Cadets,' a TV series that follows the misadventures of a ragtag team of astronauts as they navigate the perils of deep space, discovering friendship and laughter amidst the stars.",
        "Experience the suspense in 'Undercover Tales,' a movie that delves into the life of a detective going deep undercover to dismantle a crime syndicate, testing the limits of loyalty and justice."
    };
    public static readonly string[] COURSE_DESCR =
    {
        "Enroll in 'Creative Writing Essentials,' a course designed to unleash your storytelling potential. Learn from acclaimed authors about developing plots, creating characters, and building worlds that captivate readers.",
        "Master the art of digital marketing with our 'Online Influence Builder' course. Gain expertise in social media strategy, SEO, content marketing, and analytics to elevate your brand's online presence.",
        "Improve your physical and mental well-being with 'Yoga for Life,' a course offering a holistic approach to yoga. Discover various asanas, breathing techniques, and mindfulness practices to enhance your daily routine."
    };

    public static readonly string[][] PRODUCT_REVIEWS =
    {
        new[]
        {
            "Extremely disappointed. The product broke after one use. Not recommended at all.",
            "One star is too generous for this item. It's nothing like the description.",
            "Would give zero stars if I could. Completely unsatisfactory quality and service.",
            "Received it damaged and customer service was unhelpful. Avoid this product.",
            "Not fit for purpose. It's cheaply made and completely unreliable.",
            "Terrible experience. It's overpriced and under-performs. I feel misled by the advertising.",
            "The worst purchase I've ever made. It's not at all user-friendly or efficient.",
            "Stay away! It's defective and the company doesn't honor the warranty.",
            "I regret buying this. It's incredibly ineffective and a waste of money.",
            "Totally useless product. It didn't work as expected and no support was provided."
        },
        new []
        {
            "The product somewhat met my expectations, but overall it's flawed and inefficient.",
            "Slightly better than terrible. It functions, but I'm not happy with the quality.",
            "The concept is good, but the execution is lacking. Disappointing to say the least.",
            "Has potential, but poor craftsmanship led to a subpar experience.",
            "Not the worst I've seen, but I had several issues that make me reluctant to recommend it.",
            "Customer support was friendly, but that doesn't make up for the product's poor performance.",
            "It works intermittently, which is frustrating at this price point.",
            "The material quality is not as advertised. Feels cheap and fragile.",
            "Two stars because it arrived on time, but the product does not live up to expectations.",
            "Somewhat functional, but I encountered many problems during use. It's quite unreliable."
        },
        new []
        {
            "Average product, does the job but nothing to write home about.",
            "Three stars for effort, but there’s definitely room for improvement.",
            "It's okay, meets the basic requirements but I wouldn't purchase it again.",
            "Decent functionality, but it doesn't stand out in a crowded market.",
            "The product is reliable enough, but I expected a bit more for the cost.",
            "An ordinary product with a few redeeming qualities. Not the best, but not the worst.",
            "Mediocre performance - good in some aspects but falls short in others.",
            "Fairly priced and works as intended, though it lacks the wow factor.",
            "Serviceable, but there are competitors that offer more bang for your buck.",
            "Satisfactory, but it did not fully meet my expectations. Could use some enhancements."
        },
        new []
        {
            "Really good product overall, just a few minor issues that keep it from being perfect.",
            "Impressed with the quality for the price, it's a great deal.",
            "Solid performance and easy to use, though there is a slight learning curve.",
            "The product is robust and reliable, with just a few small improvements needed.",
            "I'm very satisfied. It delivers on its promises, with only a few small drawbacks.",
            "Great value for money, and the customer service was excellent.",
            "Very efficient and effective, though it could use a bit more polish in design.",
            "Exceeded my expectations in many areas, but stopped short of a full five stars due to minor issues.",
            "Works well and is durable. I would recommend it, despite a few quirks.",
            "A well-made product that serves its purpose almost perfectly. Just shy of perfect due to minor inconveniences."
        },
        new []
        {
            "Absolutely perfect! This product has exceeded all my expectations and I highly recommend it!",
            "Incredible performance and unmatched quality. I'm thoroughly impressed!",
            "Five stars! It's been a fantastic experience from start to finish with this product.",
            "Exceptional value, superior quality, and outstanding customer service. Couldn't be happier!",
            "A game-changer in its category. I'm blown away by the effectiveness of this product.",
            "Top-notch product! It's durable, reliable, and has made a significant impact on my daily life.",
            "Remarkable results! It does exactly what it promises and then some.",
            "By far the best product I've ever used. It's well worth the investment.",
            "A flawless product! It's not only efficient but also elegantly designed.",
            "This is a must-have! It has truly made a difference and I give it a wholehearted 5 stars!"
        }
    };

    public static readonly string[] NAMES =
    {
        "James Smith",
        "Maria Rodriguez",
        "Robert Johnson",
        "Linda Martinez",
        "Michael Brown",
        "Elizabeth Taylor",
        "William Jones",
        "Barbara Wilson",
        "David Lee",
        "Jennifer White",
        "Richard Harris",
        "Patricia Clark",
        "Joseph Young",
        "Sarah Lewis",
        "Charles Hall",
        "Lisa Allen",
        "Thomas Walker",
        "Nancy King",
        "Christopher Wright",
        "Susan Lopez",
        "Daniel Adams",
        "Margaret Green",
        "Matthew Hill",
        "Jessica Moore",
        "Anthony Scott",
        "Sandra Mitchell",
        "Brian Campbell",
        "Betty Carter",
        "Kevin Roberts",
        "Karen Rodriguez",
        "Laura Turner",
        "John Phillips",
        "Peter Evans",
        "Amanda Torres",
        "Justin Nelson",
        "Melissa Edwards",
        "Brandon Parker",
        "Stephanie Ross",
        "Gary Morgan",
        "Tina Perry",
        "Edward Powell",
        "Cynthia Coleman",
        "Jeffrey Sanchez",
        "Rebecca Rivera",
        "Gregory Cox",
        "Kathleen Howard",
        "Jerry Ward",
        "Shirley Brooks",
        "Walter Diaz",
        "Gloria Gray"
    };
    public static readonly string[] ISBNS =
    {
        "978-3-16-148410-0",
        "978-0-575-08358-1",
        "978-1-4028-9462-6",
        "978-0-596-52068-7",
        "978-0-306-40615-7",
        "978-3-540-49698-4",
        "978-0-387-95041-9",
        "978-1-891830-85-3",
        "978-1-60309-047-6",
        "978-0-345-54288-5",
        "978-0-671-72706-1",
        "978-1-4516-2178-2",
        "978-0-7432-7356-0",
        "978-0-141-18436-1",
        "978-0-8135-2905-4",
        "978-0-425-21256-8",
        "978-0-312-93921-9",
        "978-0-307-29152-0",
        "978-0-7434-4620-4",
        "978-0-684-85636-2",
        "978-0-06-112241-5",
        "978-0-394-58048-5",
        "978-0-345-47250-1",
        "978-0-689-86746-0",
        "978-0-553-38280-2",
        "978-0-307-26575-3",
        "978-0-446-57980-3",
        "978-0-14-017739-8",
        "978-0-06-083865-2",
        "978-0-7615-3643-7"
    };
    public static readonly string[] PUBLISHERS =
    {
        "Penguin Random House",
        "HarperCollins",
        "Simon & Schuster",
        "Hachette Book Group",
        "Macmillan Publishers",
        "Scholastic Inc.",
        "Wiley (John Wiley & Sons)",
        "McGraw-Hill Education",
        "Pearson Education",
        "Elsevier",
        "Oxford University Press",
        "Springer Nature",
        "Cengage Learning",
        "Taylor & Francis",
        "SAGE Publications",
        "Bloomsbury",
        "Routledge",
        "Cambridge University Press",
        "Thomson Reuters",
        "Houghton Mifflin Harcourt",
        "Kodansha",
        "Shueisha",
        "Klett Gruppe",
        "Planeta Group",
        "Bonnier Books",
        "Bertelsmann",
        "Wolters Kluwer",
        "Random House Publishing Group",
        "Chronicle Books",
        "Zondervan"
    };
    public static readonly string[] DIRECTORS =
    {
        "Steven Spielberg",
        "Martin Scorsese",
        "Alfred Hitchcock",
        "Christopher Nolan",
        "Quentin Tarantino",
        "Ridley Scott",
        "James Cameron",
        "David Fincher",
        "Stanley Kubrick",
        "Peter Jackson",
        "Francis Ford Coppola",
        "Guillermo del Toro",
        "Tim Burton",
        "Spike Lee",
        "Wes Anderson",
        "J.J. Abrams",
        "Michael Bay",
        "Kathryn Bigelow",
        "Clint Eastwood",
        "Ron Howard",
        "David Lynch",
        "George Lucas",
        "Sam Mendes",
        "Hayao Miyazaki",
        "Tyler Perry",
        "Guy Ritchie",
        "Oliver Stone",
        "Denis Villeneuve",
        "John Woo",
        "Zack Snyder"
    };
    public static readonly string[] ACTORS =
    {
        "Tom Hanks",
        "Meryl Streep",
        "Leonardo DiCaprio",
        "Denzel Washington",
        "Sandra Bullock",
        "Brad Pitt",
        "Julia Roberts",
        "George Clooney",
        "Angelina Jolie",
        "Johnny Depp",
        "Robert De Niro",
        "Kate Winslet",
        "Morgan Freeman",
        "Scarlett Johansson",
        "Russell Crowe",
        "Nicole Kidman",
        "Christian Bale",
        "Cate Blanchett",
        "Daniel Day-Lewis",
        "Emma Stone",
        "Will Smith",
        "Natalie Portman",
        "Matt Damon",
        "Anne Hathaway",
        "Tom Cruise",
        "Jennifer Lawrence",
        "Hugh Jackman",
        "Charlize Theron",
        "Ryan Gosling",
        "Amy Adams",
        "Joaquin Phoenix",
        "Viola Davis",
        "Mark Wahlberg",
        "Marion Cotillard",
        "Chris Hemsworth",
        "Jessica Chastain",
        "James McAvoy",
        "Margot Robbie",
        "Idris Elba",
        "Naomi Watts",
        "Benedict Cumberbatch",
        "Emily Blunt",
        "Robert Downey Jr.",
        "Rachel McAdams",
        "Jake Gyllenhaal",
        "Lupita Nyong'o",
        "Jeremy Renner",
        "Reese Witherspoon",
        "Michael Fassbender",
        "Saoirse Ronan"
    };
    public static readonly string[] SOFTWARE_VERSIONS =
    {
        "1.0.0",
        "1.0.1",
        "1.1.0",
        "1.2.3",
        "2.0.0",
        "2.1.0",
        "2.1.1",
        "2.2.0",
        "3.0.0",
        "3.0.1",
        "3.1.0",
        "3.1.5",
        "4.0.0",
        "4.1.0",
        "4.1.2",
        "4.2.0",
        "5.0.0",
        "5.1.0",
        "5.1.3",
        "5.2.0",
        "6.0.0",
        "6.1.0",
        "6.1.4",
        "6.2.0",
        "7.0.0",
        "7.1.0",
        "7.1.1",
        "7.2.0",
        "8.0.0",
        "8.1.0"
    };
    public static readonly string[] SOFTWARE_DEVELOPERS =
    {
        "Bill Gates",
        "Mark Zuckerberg",
        "Jeff Bezos",
        "Larry Page",
        "Sergey Brin",
        "Steve Wozniak",
        "Tim Berners-Lee",
        "Linus Torvalds",
        "Guido van Rossum",
        "Bjarne Stroustrup",
        "Ken Thompson",
        "Brian Kernighan",
        "Dennis Ritchie",
        "Richard Stallman",
        "John Carmack",
        "James Gosling",
        "Marc Andreessen",
        "Anders Hejlsberg",
        "Steve Jobs",
        "Elon Musk",
        "Satya Nadella",
        "Sundar Pichai",
        "Ben Silbermann",
        "Rasmus Lerdorf",
        "Jack Dorsey",
        "Evan Spiegel",
        "Reed Hastings",
        "Mitchell Baker",
        "Gabe Newell",
        "Tommy Refenes"
    };
    public static readonly string[] GAME_DEVELOPERS =
    {
        "Nintendo",
        "Valve Corporation",
        "Rockstar Games",
        "Electronic Arts",
        "Activision Blizzard",
        "Ubisoft",
        "Square Enix",
        "Bethesda Game Studios",
        "Capcom",
        "Bungie",
        "Konami",
        "Epic Games",
        "CD Projekt Red",
        "Naughty Dog",
        "FromSoftware",
        "BioWare",
        "BANDAI NAMCO Entertainment",
        "343 Industries",
        "Infinity Ward",
        "Sony Santa Monica",
        "Insomniac Games",
        "SEGA",
        "Blizzard Entertainment",
        "Treyarch",
        "Riot Games",
        "Supercell",
        "Mojang Studios",
        "Rare",
        "Obsidian Entertainment",
        "Respawn Entertainment"
    };
    public static readonly string[] SOFTWARE_DEPENDENCIES =
    {
        "Newtonsoft.Json",
        "Entity Framework",
        "xUnit",
        "NUnit",
        "Moq",
        "log4net",
        "NLog",
        "AutoMapper",
        "Dapper",
        "ASP.NET MVC",
        "jQuery",
        "React",
        "Angular",
        "Vue.js",
        "Bootstrap",
        "Node.js",
        "Express",
        "Spring Framework",
        "Hibernate",
        "TensorFlow",
        "PyTorch",
        "Pandas",
        "NumPy",
        "Ruby on Rails",
        "Flask",
        "Django",
        "Laravel",
        "Symfony",
        "PHPUnit",
        "Apache Commons"
    };
    public static readonly string[] SOFTWARE_TRIAL_LIMITATIONS =
    {
        "Time-Limited Usage",
        "Feature-Limited Access",
        "Limited Output Quality",
        "Watermark on Output",
        "Limited Data Export",
        "Limited Number of Uses",
        "Nag Screens",
        "Limited Support",
        "Restricted File Saving",
        "Limited Number of Records"
    };
    public static readonly string[] COURSE_REQUIREMENTS =
    {
        "Basic Understanding of Subject Matter",
        "Access to a Computer",
        "Internet Connectivity",
        "Specific Software Installation",
        "Prior Course Completion",
        "Textbooks or Course Materials",
        "Functional Webcam and Microphone",
        "Active Participation",
        "Completion of Pre-course Assessment",
        "Minimum Age Requirement",
        "Proficiency in Course Language",
        "Time Commitment for Duration of Course",
        "Registration or Enrollment in Institution",
        "Specific Operating System",
        "Access to a Scientific Calculator",
        "Required Reading Before Course Start",
        "Updated Web Browser",
        "Learning Management System Access",
        "Payment of Course Fee",
        "Signed Student Agreement"
    };
}