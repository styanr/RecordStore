import {
  Box,
  Container,
  Heading,
  Text,
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  useDisclosure,
  Input,
  Textarea,
  Flex,
  Spinner,
  HStack,
  InputGroup,
  InputLeftElement,
  useToast,
} from '@chakra-ui/react';
import { FaSearch } from 'react-icons/fa';
import { useState, useEffect } from 'react';
import useArtists from '../../hooks/useArtists';
import usePages from '../../hooks/usePages';

const Artists = () => {
  const toast = useToast();
  const { isOpen, onOpen, onClose } = useDisclosure();
  const { getArtists, getArtist, createArtist, updateArtist, deleteArtist } =
    useArtists();
  const [artists, setArtists] = useState([]);
  const [selectedArtist, setSelectedArtist] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [searchName, setSearchName] = useState('');

  const {
    page,
    setPage,
    totalPages,
    setTotalPages,
    nextPage,
    prevPage,
    goToPage,
  } = usePages();

  const fetchArtists = async () => {
    setIsLoading(true);
    const data = await getArtists({ page, name: searchName });
    setTotalPages(data.pageCount);
    setArtists(data);
    setIsLoading(false);
  };

  useEffect(() => {
    fetchArtists();
  }, []);

  useEffect(() => {
    fetchArtists();
  }, [page, searchName]);

  const handleViewArtist = async (id) => {
    setIsLoading(true);
    const artist = await getArtist(id);
    setSelectedArtist(artist);
    setName(artist.name);
    setDescription(artist.description);
    setIsEditing(false);
    onOpen();
    setIsLoading(false);
  };

  const handleCreateArtist = () => {
    setSelectedArtist(null);
    setName('');
    setDescription('');
    setIsEditing(true);
    onOpen();
  };

  const handleSaveArtist = async () => {
    setIsLoading(true);
    try {
      if (isEditing) {
        const newArtist = { name, description };
        const savedArtist = await createArtist(newArtist);
        setArtists({ ...artists, results: [...artists.results, savedArtist] });
      } else {
        const updatedArtist = { name, description };
        const savedArtist = await updateArtist(
          selectedArtist.id,
          updatedArtist
        );
        setArtists({
          ...artists,
          results: artists.results.map((artist) =>
            artist.id === selectedArtist.id ? savedArtist : artist
          ),
          rowCount: artists.rowCount + 1,
        });
      }
    } catch (error) {
      toast({
        title: 'Помилка збереження виконавця',
        description: error.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
    onClose();
    setIsLoading(false);
  };

  const handleDeleteArtist = async () => {
    setIsLoading(true);
    try {
      await deleteArtist(selectedArtist.id);
      setArtists({
        ...artists,
        results: artists.results.filter(
          (artist) => artist.id !== selectedArtist.id
        ),
        rowCount: artists.rowCount - 1,
      });
    } catch (error) {
      toast({
        title: 'Помилка видалення виконавця',
        description: error.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
    onClose();
    setIsLoading(false);
  };

  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Heading color='teal.600' mb={4}>
          Виконавці
        </Heading>
        <InputGroup mb={4}>
          <InputLeftElement pointerEvents='none'>
            <FaSearch color='gray.300' />
          </InputLeftElement>
          <Input
            placeholder='Пошук виконавця...'
            value={searchName}
            onChange={(e) => setSearchName(e.target.value)}
          />
        </InputGroup>
        {isLoading ? (
          <Flex justify='center' mt={8}>
            <Spinner size='xl' color='teal.500' />
          </Flex>
        ) : (
          <Box mb={4}>
            <Text>{artists.rowCount} виконавців</Text>
            <Text>{artists.pageCount} сторінок</Text>
          </Box>
        )}
        <Button colorScheme='teal' onClick={handleCreateArtist}>
          Створити виконавця
        </Button>
        {isLoading ? (
          <Flex justify='center' mt={8}>
            <Spinner size='xl' color='teal.500' />
          </Flex>
        ) : (
          <Box mt={8}>
            {artists.results.map((artist) => (
              <Box
                key={artist.id}
                bg='white'
                p={4}
                borderRadius='md'
                boxShadow='md'
                mb={4}
                cursor='pointer'
                onClick={() => handleViewArtist(artist.id)}
              >
                <Heading size='md'>{artist.name}</Heading>
              </Box>
            ))}
            <HStack mt={4} justify='center'>
              <Button
                isDisabled={page === 1}
                onClick={() => prevPage()}
                size='sm'
              >
                Попередня
              </Button>
              <Text>{page}</Text>
              <Button
                isDisabled={page === totalPages}
                onClick={() => nextPage()}
                size='sm'
              >
                Наступна
              </Button>
            </HStack>
          </Box>
        )}
      </Container>

      <Modal isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>
            {isEditing ? 'Створити виконавця' : 'Редагувати виконавця'}
          </ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Input
              mb={4}
              placeholder='Назва виконавця'
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
            <Textarea
              placeholder='Опис виконавця'
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </ModalBody>
          <ModalFooter>
            {!isEditing && (
              <Button
                colorScheme='red'
                mr={4}
                onClick={handleDeleteArtist}
                isLoading={isLoading}
              >
                Видалити
              </Button>
            )}
            <Button
              colorScheme='teal'
              onClick={handleSaveArtist}
              isLoading={isLoading}
            >
              {isEditing ? 'Створити' : 'Зберегти'}
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </Box>
  );
};

export default Artists;
